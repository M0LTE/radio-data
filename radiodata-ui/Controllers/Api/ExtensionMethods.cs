using System.Globalization;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace radiodata_ui.Controllers.Api;

internal static class ExtensionMethods
{
    public static string CapitaliseLocator(this string source)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < source.Length; i++)
        {
            if (i < 2)
            {
                sb.Append(char.ToUpper(source[i]));
            }
            else
            {
                sb.Append(char.ToLower(source[i]));
            }
        }
        return sb.ToString();
    }

    public static string ToTitleCase(this string source) => ToTitleCase(source, null);

    public static string ToTitleCase(this string source, CultureInfo? culture)
    {
        culture = culture ?? CultureInfo.CurrentUICulture;
        return culture.TextInfo.ToTitleCase(source.ToLower());
    }
}