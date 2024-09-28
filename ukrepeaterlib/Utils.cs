namespace ukrepeaterlib;

internal static class Utils
{
    public static string To8CharNgr(string lowPrecisionNgr)
    {
        if (lowPrecisionNgr.Length > 7) return lowPrecisionNgr;

        var firstTwo = $"{lowPrecisionNgr[0]}{lowPrecisionNgr[1]}";
        var secondTwo = $"{lowPrecisionNgr[2]}{lowPrecisionNgr[3]}";
        var thirdTwo = $"{lowPrecisionNgr[4]}{lowPrecisionNgr[5]}";

        var output = $"{firstTwo}{secondTwo}0{thirdTwo}0";
        return output;
    }

    internal static LatLon? Round(double lat, double lon)
    {
        var _location = new LatLon
        {
            Latitude = Math.Round(lat, 6),
            Longitude = Math.Round(lon, 6)
        };

        return _location;
    }

    internal static bool IsLocator(string locator)
    {
        if (string.IsNullOrWhiteSpace(locator))
        {
            return false;
        }

        if (locator.Length % 2 != 0)
        {
            return false;
        }

        if (locator.Length < 4)
        {
            return false;
        }

        // IO
        if (!IsAlpha(locator[0]) || !IsAlpha(locator[1]))
        {
            return false;
        }

        // IO91
        if (!IsNumber(locator[2]) || !IsNumber(locator[3]))
        {
            return false;
        }

        // IO91lk
        if (locator.Length > 4)
        {
            if (!IsAlpha(locator[4]) || !IsAlpha(locator[5]))
            {
                return false;
            }
        }

        // IO91lk23
        if (locator.Length > 6)
        {
            if (!IsNumber(locator[6]) || !IsNumber(locator[7]))
            {
                return false;
            }
        }

        if (locator.Length > 8)
        {
            throw new NotImplementedException("Locators over 8 chars not supported");
        }

        return true;
    }

    internal static bool IsNgr(string? ngr)
    {
        if (ngr == null)
        {
            return false;
        }

        if (ngr.Length % 2 != 0)
        {
            return false;
        }

        if (ngr.Length < 4)
        {
            return false;
        }

        if (ngr.StartsWith("I"))
        {
            return false;
        }

        if (!IsAlpha(ngr[0]))
        {
            return false;
        }

        if (!IsAlpha(ngr[1]))
        {
            return false;
        }

        if (!ngr[2..].ToCharArray().All(IsNumber))
        {
            return false;
        }

        return true;
    }

    private static bool IsNumber(char c) => c is >= '0' and <= '9';

    private static bool IsAlpha(char c) => (c is >= 'A' and <= 'Z') || (c is >= 'a' and <= 'z');
}
