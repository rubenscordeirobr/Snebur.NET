using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Snebur.Core.Utils;

public static class PathUtils
{
    [return: NotNullIfNotNull(nameof(path))]
    public static string? RemoveInvalidPathChars(string? path)
    {
        if (path is null)
            return null;

        var invalidChars = Path.GetInvalidPathChars();
        var sb = new StringBuilder(path.Length);
        foreach (var c in path)
        {
            if (!invalidChars.Contains(c))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();

    }

    [return: NotNullIfNotNull(nameof(path))]
    public static string? RemoveExtension(string? path)
    {
        if (path is null)
            return null;
         
        var fileName = Path.GetFileNameWithoutExtension(path);
        var directoryName = Path.GetDirectoryName(path);
        if (directoryName is null)
            return fileName;

        return Path.Combine(directoryName, fileName);
    }
}

