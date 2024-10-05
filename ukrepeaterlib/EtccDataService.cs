namespace ukrepeaterlib;

public class EtccDataService
{
    public async Task<List<EtccRecord>> GetVhfAndUhfAnalogueTargets(string locator, bool includePersonalCalls, int? km)
    {
        var client = new EtccApiClient();

        var data = await client.GetAll();

        var vhfAndUhfRepeaters = data
            .Where(r => includePersonalCalls == true || r.Repeater.StartsWith("GB") || r.Repeater.StartsWith("MB"))
            .Where(r => r.Status == "OPERATIONAL")
            .Where(r => r.Band == "2M" || r.Band == "70CM")
            .Where(r => r.Type == "" || EtccRepeaterType.TryParse(r.Type, out var type) && type.IsVoice)
            .Where(r => (r.ModeCodes.Length == 0 && r.Type == EtccRepeaterType.AnalogueGateway)
                        || r.ModeCodes.Contains(EtccModeFlag.Analogue))
            .Where(r => km == null || r.DistanceFrom(locator) <= km)
            .OrderBy(r => r.DistanceFrom(locator) ?? double.MaxValue)
            .ToList();

        return vhfAndUhfRepeaters;
    }
}
