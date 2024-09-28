using chirpcsvlib;
using ukrepeaterlib;

namespace ukrepeaterlib_tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var client = new EtccApiClient();

            var data = await client.GetAll();

            var vhfAndUhfRepeaters = data
                .Where(r => r.Band == "2M" || r.Band == "70CM")
                .Where(r => r.Type == "AV")
                .Where(r => r.ModeCodes.Contains(EtccModeFlag.Analogue))
                .OrderBy(r => r.DistanceFrom("IO91lk") ?? double.MaxValue)
                .ToList();

            int i = 0;
            var chirpRows = vhfAndUhfRepeaters
                .Select(r => r.ToChirpCsvRow(i++))
                .Where(r => r != null)
                .Select(r => r!);

            var csv = ChirpCsvFileUtils.ToCsv(chirpRows);

            File.WriteAllText("output.csv", csv);
        }
    }
}