using System.Net.Http.Json;

namespace ukrepeaterlib;

public class EtccApiClient(HttpClient httpClient)
{
    public EtccApiClient() : this(new HttpClient()) { }

    public async Task<ICollection<EtccRecord>> GetAll()
    {
        var response = await httpClient.GetAsync("https://api-beta.rsgb.online/all/systems");
        response.EnsureSuccessStatusCode();
        var repeaters = await response.Content.ReadFromJsonAsync<EtccApiResponse>();
        return repeaters?.Data ?? [];
    }
}

