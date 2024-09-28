﻿namespace ukrepeaterlib;

public record EtccRecord
{
    private string repeater = "";

    public int Id { get; set; }
    public bool Fac { get; set; }
    public string Type { get; set; } = "";
    public string Status { get; set; } = "";
    public string KeeperCallsign { get; set; } = "";
    public string Town { get; set; } = "";
    public string[] ModeCodes { get; set; } = [];
    public long Tx { get; set; }
    public string Repeater { get => repeater; set => repeater = value.Trim(); }
    public long Rx { get; set; }
    public decimal Ctcss { get; set; }
    public decimal Txbw { get; set; }
    public string Band { get; set; } = "";
    public string Locator { get; set; } = "";
    public decimal DbwErp { get; set; }
    public ExtraDetails ExtraDetails { get; set; } = new();
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