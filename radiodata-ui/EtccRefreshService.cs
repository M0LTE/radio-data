using ukrepeaterlib;

namespace radiodata_ui;

/// <summary>
/// Warms the ETCC cache at startup and refreshes it periodically, so requests are served
/// from cache (fast) and the site recovers automatically when the upstream API returns —
/// no user request required to trigger a refresh.
/// </summary>
public sealed class EtccRefreshService(EtccRepository repository, ILogger<EtccRefreshService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        try
        {
            do
            {
                try
                {
                    await repository.RefreshAsync(stoppingToken);
                }
                catch (EtccDataUnavailableException)
                {
                    // Upstream still down and nothing cached yet; already logged. Retry next tick.
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error refreshing ETCC cache.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // Application is shutting down.
        }
    }
}
