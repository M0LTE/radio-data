using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ukrepeaterlib;

/// <summary>
/// Fetches the ETCC systems list and caches the last-good result so the site keeps
/// working through upstream outages. Behaviour:
/// <list type="bullet">
/// <item>serves the in-memory snapshot while it is fresh (no network hit);</item>
/// <item>single-flights refreshes — concurrent callers share one upstream fetch;</item>
/// <item>falls back to the last-good (stale) snapshot when the upstream is unreachable;</item>
/// <item>throws <see cref="EtccDataUnavailableException"/> only when there is no data at all;</item>
/// <item>backs off briefly after a failure so an outage doesn't make every request wait;</item>
/// <item>best-effort persists the last-good snapshot to disk so it survives a restart.</item>
/// </list>
/// Registered as a singleton so the cache is shared across requests.
/// </summary>
public sealed class EtccRepository
{
    private static readonly TimeSpan FreshFor = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan FailureBackoff = TimeSpan.FromSeconds(30);

    private readonly Func<CancellationToken, Task<ICollection<EtccRecord>>> _fetch;
    private readonly string? _cachePath;
    private readonly ILogger? _logger;
    private readonly object _lock = new();
    private volatile Snapshot? _snapshot;
    private Task<ICollection<EtccRecord>>? _inFlight;   // guarded by _lock
    private DateTimeOffset _lastFailureUtc = DateTimeOffset.MinValue;   // guarded by _lock

    private sealed record Snapshot(DateTimeOffset FetchedAt, ICollection<EtccRecord> Data);

    private sealed record PersistedCache(DateTimeOffset FetchedAt, EtccRecord[] Data);

    /// <param name="fetch">Delegate that fetches the systems list from the upstream API.</param>
    /// <param name="cachePath">Optional path to persist the last-good snapshot to; null/empty disables disk persistence.</param>
    public EtccRepository(
        Func<CancellationToken, Task<ICollection<EtccRecord>>> fetch,
        string? cachePath = null,
        ILogger<EtccRepository>? logger = null)
    {
        _fetch = fetch;
        _cachePath = string.IsNullOrWhiteSpace(cachePath) ? null : cachePath;
        _logger = logger;
        TryLoadFromDisk();
    }

    /// <summary>Convenience for callers not using DI (e.g. tests): a direct API client, no disk cache.</summary>
    public EtccRepository(EtccApiClient client, string? cachePath = null, ILogger<EtccRepository>? logger = null)
        : this(client.GetAll, cachePath, logger) { }

    public bool HasData => _snapshot is not null;

    public DateTimeOffset? LastUpdatedUtc => _snapshot?.FetchedAt;

    /// <summary>Returns cached data if fresh, otherwise refreshes (falling back to stale on failure).</summary>
    public Task<ICollection<EtccRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = _snapshot;
        if (snapshot is not null && DateTimeOffset.UtcNow - snapshot.FetchedAt < FreshFor)
            return Task.FromResult(snapshot.Data);

        return RefreshAsync(cancellationToken);
    }

    /// <summary>Refreshes from the upstream, serving stale data if the fetch fails and any exists.</summary>
    public Task<ICollection<EtccRecord>> RefreshAsync(CancellationToken cancellationToken = default)
    {
        Task<ICollection<EtccRecord>> inFlight;
        lock (_lock)
        {
            var snapshot = _snapshot;
            if (snapshot is not null && DateTimeOffset.UtcNow - snapshot.FetchedAt < FreshFor)
                return Task.FromResult(snapshot.Data);

            // Recently failed and nobody is already fetching: don't make this caller wait on the
            // upstream again. Serve stale if we have it, otherwise fail fast.
            if (_inFlight is null && DateTimeOffset.UtcNow - _lastFailureUtc < FailureBackoff)
            {
                if (snapshot is not null) return Task.FromResult(snapshot.Data);
                return Task.FromException<ICollection<EtccRecord>>(new EtccDataUnavailableException(
                    "The ETCC repeater API is currently unavailable and no cached data is available."));
            }

            // Join the in-flight fetch if there is one, otherwise start it.
            _inFlight ??= FetchAndCacheAsync();
            inFlight = _inFlight;
        }

        // Let a caller's cancellation abandon its own wait without cancelling the shared fetch.
        return cancellationToken.CanBeCanceled ? inFlight.WaitAsync(cancellationToken) : inFlight;
    }

    private async Task<ICollection<EtccRecord>> FetchAndCacheAsync()
    {
        // Never complete synchronously while a caller holds _lock (the finally re-enters it).
        await Task.Yield();
        try
        {
            var data = await _fetch(CancellationToken.None);
            var snapshot = new Snapshot(DateTimeOffset.UtcNow, data);
            _snapshot = snapshot;
            TrySaveToDisk(snapshot);
            _logger?.LogInformation("Fetched {Count} ETCC systems from upstream.", data.Count);
            return data;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            lock (_lock) { _lastFailureUtc = DateTimeOffset.UtcNow; }

            var stale = _snapshot;
            if (stale is not null)
            {
                _logger?.LogWarning(ex, "ETCC upstream unavailable; serving cached data from {FetchedAt:o}.", stale.FetchedAt);
                return stale.Data;
            }

            _logger?.LogError(ex, "ETCC upstream unavailable and no cached data to fall back on.");
            throw new EtccDataUnavailableException(
                "The ETCC repeater API is currently unavailable and no cached data is available.", ex);
        }
        finally
        {
            lock (_lock) { _inFlight = null; }
        }
    }

    private void TryLoadFromDisk()
    {
        if (_cachePath is null || !File.Exists(_cachePath)) return;
        try
        {
            var cache = JsonSerializer.Deserialize<PersistedCache>(File.ReadAllText(_cachePath));
            if (cache is { Data.Length: > 0 })
            {
                _snapshot = new Snapshot(cache.FetchedAt, cache.Data);
                _logger?.LogInformation("Loaded {Count} ETCC systems from disk cache dated {FetchedAt:o}.", cache.Data.Length, cache.FetchedAt);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to load ETCC disk cache from {Path}.", _cachePath);
        }
    }

    private void TrySaveToDisk(Snapshot snapshot)
    {
        if (_cachePath is null) return;
        try
        {
            var dir = Path.GetDirectoryName(_cachePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(new PersistedCache(snapshot.FetchedAt, snapshot.Data.ToArray()));
            // Write-then-rename so a crash mid-write can't leave a truncated cache file.
            var tmp = _cachePath + ".tmp";
            File.WriteAllText(tmp, json);
            File.Move(tmp, _cachePath, overwrite: true);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to persist ETCC disk cache to {Path}.", _cachePath);
        }
    }
}
