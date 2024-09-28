
namespace ukrepeaterlib;

public readonly record struct EtccRepeaterType(string Type, string Description, bool IsVoice = false, bool IsData = false)
{
    public static readonly EtccRepeaterType AnalogueGateway = new("AG", "Analogue Gateway", IsVoice: true);
    public static readonly EtccRepeaterType Aprs = new("AP", "APRS", IsData: true);
    public static readonly EtccRepeaterType AnalogueVoice = new("AV", "Analogue Voice", IsVoice: true);
    public static readonly EtccRepeaterType Beacon = new("BN", "Beacon");
    public static readonly EtccRepeaterType DigitalData = new("DD", "Digital Data", IsData: true);
    public static readonly EtccRepeaterType DigitalGateway = new("DG", "Digital Gateway");
    public static readonly EtccRepeaterType DualMode = new("DM", "Dual Mode", IsVoice: true);
    public static readonly EtccRepeaterType DigitalVoice = new("DV", "Digital Voice", IsVoice: true);
    public static readonly EtccRepeaterType PacketNode = new("PN", "Packet Node", IsData: true);
    public static readonly EtccRepeaterType PacketMailbox = new("PX", "Packet Mailbox", IsData: true);
    public static readonly EtccRepeaterType RegenNode = new("RN", "Regenerative Node");
    public static readonly EtccRepeaterType RepeaterLink = new("RL", "Repeater Link");
    public static readonly EtccRepeaterType Special = new("SP", "Special");
    public static readonly EtccRepeaterType TelevisionRepeater = new("TV", "Television Repeater");

    public static readonly EtccRepeaterType[] All =
    [
        AnalogueGateway,
        Aprs,
        AnalogueVoice,
        Beacon,
        DigitalData,
        DigitalGateway,
        DualMode,
        DigitalVoice,
        PacketNode,
        PacketMailbox,
        RegenNode,
        RepeaterLink,
        Special,
        TelevisionRepeater
    ];

    public string TypeCode { get; } = Type;
    public string Description { get; } = Description;

    public static bool TryParse(string text, out EtccRepeaterType type)
    {
        foreach (var t in All)
        {
            if (t.TypeCode == text)
            {
                type = t;
                return true;
            }
        }

        type = default;
        return false;
    }

    public static implicit operator string(EtccRepeaterType repeaterType) => repeaterType.TypeCode;
}
