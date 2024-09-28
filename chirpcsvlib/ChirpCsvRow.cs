using CsvHelper.Configuration.Attributes;

namespace chirpcsvlib;

public record ChirpCsvRow
{
    /// <summary>
    /// e.g. 0
    /// </summary>
    public required int Location { get; set; }

    /// <summary>
    /// e.g. GB3BN
    /// </summary>
    [Name("Name")]
    public required string Name { get; set; }

    /// <summary>
    /// e.g. 433.000000
    /// </summary>
    [Name("Frequency")]
    public required decimal Frequency { get; set; }

    /// <summary>
    /// e.g. +
    /// </summary>
    [Name("Duplex")]
    public required char Duplex { get; set; }

    /// <summary>
    /// e.g. 1.6
    /// </summary>
    [Name("Offset")]
    public required decimal Offset { get; set; }

    /// <summary>
    /// e.g. Tone
    /// </summary>
    [Name("Tone")]
    public string? Tone { get; set; }

    /// <summary>
    /// e.g. 118.8
    /// </summary>
    [Name("rToneFreq")]
    public decimal? RToneFreq { get; set; }

    /// <summary>
    /// e.g. FM
    /// </summary>
    [Name("Mode")]
    public required string Mode { get; set; }

    [Name("TStep")]
    public required double TStep { get; set; }

    [Name("Power")]
    public required string Power { get; set; }

    [Name("Comment")]
    public string? Comment { get; set; }
}
