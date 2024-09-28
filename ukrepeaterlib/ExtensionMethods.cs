using chirpcsvlib;
using DotNetCoords;
using DotNetCoords.Datum;
using MaidenheadLib;
using System.Text;
using static ukrepeaterlib.Utils;

namespace ukrepeaterlib;

public static class ExtensionMethods
{
    public static LatLon? GetPosition(this EtccRecord repeater)
    {
        if (IsNgr(repeater.ExtraDetails?.Ngr))
        {
            try
            {
                var osRef = new OSRef(To8CharNgr(repeater.ExtraDetails?.Ngr!));
                var latLng = osRef.ToLatLng();
                latLng.ToDatum(WGS84Datum.Instance);
                return Round(latLng.Latitude, latLng.Longitude);
            }
            catch
            {
            }
        }

        if (IsLocator(repeater.Locator))
        {
            try
            {
                var (lat, lon) = MaidenheadLocator.LocatorToLatLng(repeater.Locator);
                return Round(lat, lon);
            }
            catch
            {
            }
        }

        return null;
    }

    public static double? DistanceFrom(this EtccRecord repeater, string myLocator)
    {
        if (IsLocator(myLocator))
        {
            var (myLat, myLon) = MaidenheadLocator.LocatorToLatLng(myLocator);

            return DistanceFrom(repeater, (myLat, myLon));
        }

        return null;
    }

    public static double? DistanceFrom(this EtccRecord repeater, (double lat, double lon) myPosition)
    {
        var repeaterPos = repeater.GetPosition();

        if (repeaterPos == null)
        {
            return null;
        }

        return MaidenheadLocator.Distance((repeaterPos.Value.Latitude, repeaterPos.Value.Longitude), (myPosition.lat, myPosition.lon));
    }

    public static ChirpCsvRow? ToChirpCsvRow(this EtccRecord repeater, string power = "4.0W", string commentSuffix = "")
    {
        var result = new ChirpCsvRow
        {
            Location = 0,
            Name = repeater.Repeater,
            Frequency = repeater.Tx / 1000000.0M,
            Duplex = repeater.Rx > repeater.Tx ? '+' : '-',
            Offset = repeater.Offset,
            Mode = GetMode(repeater),
            Comment = GetComment(repeater, commentSuffix),
            TStep = repeater.Band == "2M" ? 12.5 : 25,
            Power = power,
        };

        if (repeater.ModeCodes.Contains(EtccModeFlag.Analogue))
        {
            if (repeater.Ctcss != 0)
            {
                result.Tone = "Tone";
                result.RToneFreq = repeater.Ctcss;
            }
            else
            {
                result.RToneFreq = 67; // lowest valid tone, not used
            }
        }
        else
        {
            return null;
        }

        return result;
    }

    private static string GetComment(EtccRecord repeater, string commentSuffix)
    {
        var sb = new StringBuilder();
        sb.Append(repeater.Town?.Trim()!);

        if (!string.IsNullOrWhiteSpace(commentSuffix))
        {
            sb.Append(" (");
            sb.Append(commentSuffix);
            sb.Append(")");
        }

        return sb.ToString();
    }

    private static string GetMode(EtccRecord repeater)
    {
        if (repeater.ModeCodes.Length == 0 || repeater.ModeCodes.Contains(EtccModeFlag.Analogue))
        {
            if (repeater.Txbw == 12.5M)
            {
                return "NFM";
            }
            else if (repeater.Txbw == 25M)
            {
                return "FM";
            }
        }

        throw new NotImplementedException();
    }
}

public readonly record struct LatLon
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
