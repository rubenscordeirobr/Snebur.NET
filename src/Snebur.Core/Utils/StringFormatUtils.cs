using System.Text.RegularExpressions;

namespace Snebur.Core.Utils;

public static class StringFormatUtils
{
    private static readonly Regex PlaceholderRegex = new(@"\{(\w+)\}", RegexOptions.Compiled);

    public static string Format(string format, params object[] args)
    {
        Guard.NotNull(format);

        if (args?.Length > 0)
        {
            return PlaceholderRegex.Replace(format, match =>
            {
                var index = Math.Min(args.Length - 1, match.Index);
                return args[index].ToString() ?? string.Empty;
            });
        }
        return format;
    }
}

