using CsvHelper;
using System.Globalization;
using System.Text;

namespace chirpcsvlib;

public static class ChirpCsvFileUtils
{
    public static string ToCsv(IEnumerable<ChirpCsvRow> rows)
    {
        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(rows);
        return stringBuilder.ToString();
    }
}
