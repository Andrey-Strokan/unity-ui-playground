using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

/// <summary>
/// Utility functions for strings.
/// </summary>
public static class StringUtility
{
    private static readonly Regex pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
    private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

    /// <summary>
    /// Convert any string to a valid C# camel case identifier.
    /// </summary>
    public static string ToCamelCase(string str)
    {
        var matchArray = pattern.Matches(str);

        StringBuilder builder = new StringBuilder();

        foreach (Match match in matchArray)
        {
            builder.Append(match.Value);
            builder.Append(" ");
        }

        var result = textInfo.ToTitleCase(builder.ToString());

        result = result.Replace(@" ", "");

        return result;
    }
}
