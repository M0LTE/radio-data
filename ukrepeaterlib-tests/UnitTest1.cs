using chirpcsvlib;
using ukrepeaterlib;

namespace ukrepeaterlib_tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            const string locator = "IO91lk";
            var service = new EtccDataService();

            var vhfAndUhfRepeaters = (await service.GetVhfAndUhfAnalogueTargets(locator))
                .Where(r => r.Band == "2M" || r.Band == "70CM")
                .Where(r => r.Type == "" || EtccRepeaterType.TryParse(r.Type, out var type) && type.IsVoice)
                .Where(r => (r.ModeCodes.Length == 0 && r.Type == EtccRepeaterType.AnalogueGateway)
                            || r.ModeCodes.Contains(EtccModeFlag.Analogue))
                .Where(r => r.Repeater == "GB3EZ")
                .ToList();

            int i = 0;
            var chirpRows = vhfAndUhfRepeaters
                .Select(r => r.ToChirpCsvRow(commentSuffix: $"{r.DistanceFrom(locator):0}km"))
                .Where(r => r != null)
                .Select(r => r!)
                .Select(r => r with { Location = i++ });

            var csv = ChirpCsvFileUtils.ToCsv(chirpRows);

            File.WriteAllText("output.csv", csv);
        }
    }
}