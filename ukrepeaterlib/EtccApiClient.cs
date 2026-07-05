using System.Net.Http.Json;

namespace ukrepeaterlib;

public class EtccApiClient(HttpClient httpClient)
{
    public const string SystemsUrl = "https://api-beta.rsgb.online/all/systems";

    // Fallback for callers that don't get an HttpClient from DI (e.g. tests). A bounded
    // timeout stops us hanging for the HttpClient default of 100s when the upstream is down.
    public EtccApiClient() : this(new HttpClient { Timeout = TimeSpan.FromSeconds(15) }) { }

    public async Task<ICollection<EtccRecord>> GetAll(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(SystemsUrl, cancellationToken);
        response.EnsureSuccessStatusCode();
        var repeaters = await response.Content.ReadFromJsonAsync<EtccApiResponse>(cancellationToken);
        return repeaters?.Data ?? [];
    }
}
