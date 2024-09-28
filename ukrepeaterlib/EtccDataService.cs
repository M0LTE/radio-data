namespace ukrepeaterlib;

public class EtccDataService
{
    public async Task<List<EtccRecord>> GetVhfAndUhfAnalogueTargets(string locator)
    {
        var client = new EtccApiClient();

        var data = await client.GetAll();

        var vhfAndUhfRepeaters = data
            .Where(r => r.Status == "OPERATIONAL")
            .Where(r => r.Band == "2M" || r.Band == "70CM")
            .Where(r => r.Type == "" || EtccRepeaterType.TryParse(r.Type, out var type) && type.IsVoice)
            .Where(r => (r.ModeCodes.Length == 0 && r.Type == EtccRepeaterType.AnalogueGateway)
                        || r.ModeCodes.Contains(EtccModeFlag.Analogue))
            .OrderBy(r => r.DistanceFrom(locator) ?? double.MaxValue)
            .ToList();

        return vhfAndUhfRepeaters;
    }
}