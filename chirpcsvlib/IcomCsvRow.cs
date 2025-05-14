using CsvHelper.Configuration.Attributes;

namespace chirpcsvlib;

public record IcomCsvRow
{
    [Name("Group No")]
    public required int GroupNo { get; set; }

    /// <summary>
    /// e.g. UK_DStar_UHF_Rpt
    /// </summary>
    [Name("Group Name")]
    public required string GroupName { get; set; }

    /// <summary>
    /// e.g. Ballycastle
    /// </summary>
    [Name("Name")]
    public required string Name { get; set; }

    /// <summary>
    /// e.g. NI-DsRp
    /// </summary>
    [Name("Sub Name")]
    public required string SubName { get; set; }

    /// <summary>
    /// e.g. GB7AH  B
    /// </summary>
    [Name("Repeater Call Sign")]
    public required string RepeaterCallsign { get; set; }

    /// <summary>
    /// e.g. GB7AH  G
    /// </summary>
    [Name("Gateway Call Sign")]
    public required string GatewayCallsign { get; set; }

    /// <summary>
    /// e.g. 439.5125
    /// </summary>
    [Name("Frequency")]
    public required decimal Frequency { get; set; }

    /// <summary>
    /// e.g. DUP-
    /// </summary>
    [Name("Dup")]
    public required string Duplex { get; set; }

    /// <summary>
    /// e.g. 9
    /// </summary>
    [Name("Offset")]
    public required decimal Offset { get; set; }

    /// <summary>
    /// e.g. DV
    /// </summary>
    [Name("Mode")]
    public required string Mode { get; set; }

    /// <summary>
    /// e.g. Tone
    /// </summary>
    [Name("TONE")]
    public string? Tone { get; set; }

    /// <summary>
    /// ?
    /// </summary>
    [Name("Repeater Tone")]
    public decimal? RToneFreq { get; set; }

    /// <summary>
    /// e.g. YES
    /// </summary>
    [Name("RPT1USE")]
    public required string Rpt1Use { get; set; }

    /// <summary>
    /// e.g. Approximate
    /// </summary>
    [Name("Position")]
    public required string Position { get; set; }

    [Name("latitude")]
    public required decimal Latitude { get; set; }

    [Name("longitude")]
    public required decimal Longitude { get; set; }

    /// <summary>
    /// e.g. 0:00
    /// </summary>
    [Name("UTC Offset")]
    public required string UtcOffset { get; set; }
}
