using ukrepeaterlib;

namespace ukrepeaterlib_tests;

public class EtccRepositoryTests
{
    private static EtccRecord Rec(string call) => new() { Repeater = call };

    [Fact]
    public async Task ReturnsFetchedData_WhenUpstreamSucceeds()
    {
        var repo = new EtccRepository(_ => Task.FromResult<ICollection<EtccRecord>>([Rec("GB3EZ")]));

        var data = await repo.GetAllAsync();

        Assert.Single(data);
        Assert.True(repo.HasData);
    }

    [Fact]
    public async Task ServesStaleData_WhenUpstreamFailsAfterAPriorSuccess()
    {
        var succeed = true;
        var repo = new EtccRepository(_ => succeed
            ? Task.FromResult<ICollection<EtccRecord>>([Rec("GB3EZ")])
            : throw new HttpRequestException("upstream down"));

        // Prime the cache.
        await repo.GetAllAsync();

        // Upstream now fails; a forced refresh must fall back to the cached snapshot.
        succeed = false;
        var data = await repo.RefreshAsync();

        Assert.Single(data);
        Assert.Equal("GB3EZ", data.First().Repeater);
    }

    [Fact]
    public async Task Throws_WhenUpstreamFailsAndNothingCached()
    {
        var repo = new EtccRepository(_ => throw new HttpRequestException("upstream down"));

        await Assert.ThrowsAsync<EtccDataUnavailableException>(() => repo.GetAllAsync());
    }

    [Fact]
    public async Task Throws_WhenUpstreamTimesOutAndNothingCached()
    {
        // The timeout path surfaces as TaskCanceledException, which must also be treated as unavailable.
        var repo = new EtccRepository(_ => throw new TaskCanceledException("timed out"));

        await Assert.ThrowsAsync<EtccDataUnavailableException>(() => repo.GetAllAsync());
    }

    [Fact]
    public async Task ConcurrentCallers_ShareASingleUpstreamFetch()
    {
        var calls = 0;
        var release = new TaskCompletionSource();
        var repo = new EtccRepository(async _ =>
        {
            Interlocked.Increment(ref calls);
            await release.Task;
            return (ICollection<EtccRecord>)[Rec("GB3EZ")];
        });

        var a = repo.GetAllAsync();
        var b = repo.GetAllAsync();
        release.SetResult();
        await Task.WhenAll(a, b);

        Assert.Equal(1, calls); // both callers shared one fetch, not one each
    }

    [Fact]
    public async Task PersistsToDisk_AndReloadsOnNextInstance()
    {
        var path = Path.Combine(Path.GetTempPath(), $"etcc-cache-test-{Guid.NewGuid():N}.json");
        try
        {
            var writer = new EtccRepository(_ => Task.FromResult<ICollection<EtccRecord>>([Rec("GB3EZ")]), path);
            await writer.GetAllAsync();
            Assert.True(File.Exists(path));

            // A fresh instance whose upstream is dead should still serve the disk-cached data.
            var reader = new EtccRepository(_ => throw new HttpRequestException("upstream down"), path);
            Assert.True(reader.HasData);
            var data = await reader.GetAllAsync();
            Assert.Equal("GB3EZ", data.First().Repeater);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
