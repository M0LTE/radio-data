using CsvHelper;
using System.Globalization;
using System.Text;

namespace chirpcsvlib;

public static class RadioCsvFileUtils
{
    public static string ToCsv(IEnumerable<ChirpCsvRow> rows)
    {
        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(rows);
        return stringBuilder.ToString();
    }

    public static string ToCsv(IEnumerable<IcomCsvRow> rows)
    {
        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(rows);
        return stringBuilder.ToString();
    }
}
