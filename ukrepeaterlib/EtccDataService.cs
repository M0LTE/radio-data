namespace ukrepeaterlib;

public class EtccDataService
{
    public async Task<IEnumerable<EtccRecord>> GetVhfAndUhfTargets(string locator, bool includePersonalCalls, int? km)
    {
        var client = new EtccApiClient();

        var data = await client.GetAll();

        var vhfAndUhfRepeaters = data
            .Where(r => includePersonalCalls == true || r.Repeater.StartsWith("GB") || r.Repeater.StartsWith("MB"))
            .Where(r => r.Status == "OPERATIONAL")
            .Where(r => r.Band == "2M" || r.Band == "70CM")
            .Where(r => r.Type == "" || EtccRepeaterType.TryParse(r.Type, out var type) && type.IsVoice)
            .Where(r => km == null || r.DistanceFrom(locator) <= km)
            .OrderBy(r => r.DistanceFrom(locator) ?? double.MaxValue);

        return vhfAndUhfRepeaters;
    }

    public async Task<IEnumerable<EtccRecord>> GetVhfAndUhfAnalogueTargets(string locator, bool includePersonalCalls, int? km)
    {
        return (await GetVhfAndUhfTargets(locator, includePersonalCalls, km))
            .Where(r => (r.ModeCodes.Length == 0 && r.Type == EtccRepeaterType.AnalogueGateway)
                || r.ModeCodes.Contains(EtccModeFlag.Analogue));
    }

    public async Task<IEnumerable<EtccRecord>> GetDstarTargets(string locator, bool includePersonalCalls, int? km)
    {
        var targets = (await GetVhfAndUhfTargets(locator, includePersonalCalls, km))
            .Where(r => r.ModeCodes.Contains("D") && r.Type == EtccRepeaterType.DigitalVoice);
        return targets;
    }
}
