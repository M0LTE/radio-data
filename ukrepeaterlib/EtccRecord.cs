namespace ukrepeaterlib;

public record EtccRecord
{
    private string repeater = "";
    private string[] modeCodes = [];
    private string town = "";

    public int Id { get; set; }
    public bool Fac { get; set; }
    public string Type { get; set; } = "";
    public string Status { get; set; } = "";
    public string KeeperCallsign { get; set; } = "";
    public string Town { get => town; set => town = value.Trim(); }
    public string[] ModeCodes { get => modeCodes; set => modeCodes = value ?? []; }
    public long Tx { get; set; }
    public string Repeater { get => repeater; set => repeater = value.Trim(); }
    public long Rx { get; set; }
    public decimal Ctcss { get; set; }
    public decimal Txbw { get; set; }
    public string Band { get; set; } = "";
    public string Locator { get; set; } = "";
    public decimal DbwErp { get; set; }
    public ExtraDetails ExtraDetails { get; set; } = new();

    public decimal Offset => Rx == 0 || Tx == 0 ? 0 : Math.Abs(Rx - Tx) / 1000000.0M;
}

public record EtccApiResponse
{
    public EtccRecord[] Data { get; set; } = [];
}

public record ExtraDetails
{
    public string Ngr { get; set; } = "";
    public int AntennaHeight { get; set; }
    public string Polarisation { get; set; } = "";
}