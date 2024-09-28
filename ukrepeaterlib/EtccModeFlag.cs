namespace ukrepeaterlib;

public readonly record struct EtccModeFlag(string Flag, EtccMode Mode, bool IsVoice = true, bool IsData = true)
{
    public static readonly EtccModeFlag Analogue = new("A", EtccMode.Analogue, IsVoice: true);
    public static readonly EtccModeFlag DSTAR = new("D", EtccMode.DSTAR, IsVoice: true);
    public static readonly EtccModeFlag Tetra = new("E", EtccMode.Tetra, IsVoice: true);
    public static readonly EtccModeFlag DMR = new("M", EtccMode.DMR, IsVoice: true);
    public static readonly EtccModeFlag Fusion = new("F", EtccMode.Fusion, IsVoice: true);
    public static readonly EtccModeFlag P25 = new("P", EtccMode.P25, IsVoice: true);
    public static readonly EtccModeFlag M17 = new("7", EtccMode.M17, IsVoice: true);
    public static readonly EtccModeFlag NXDN = new("N", EtccMode.NXDN, IsVoice: true);
    public static readonly EtccModeFlag PacketMailbox = new("PX", EtccMode.PacketMailbox, IsData: true);
    public static readonly EtccModeFlag RegenerativeNode = new("X", EtccMode.RegenerativeNode, IsData: true);

    public string Flag { get; } = Flag;
    public EtccMode Mode { get; } = Mode;
    public bool IsVoice { get; } = IsVoice;
    public bool IsData { get; } = IsData;

    public static implicit operator string(EtccModeFlag modeFlag) => modeFlag.Flag;
}

public enum EtccMode
{
    Analogue,
    DSTAR,
    Tetra,
    DMR,
    Fusion,
    P25,
    M17,
    NXDN,
    PacketMailbox,
    RegenerativeNode
}