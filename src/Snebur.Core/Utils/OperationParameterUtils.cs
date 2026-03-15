namespace Snebur.Core.Utils;

public static class OperationParameterUtils
{
    public static string NormalizeKey(string? key)
    {
        Guard.NotNullOrWhiteSpace(key);

        var index = 0;
        while (char.IsUpper(key[index]))
        {
            index++;
        }
        if (index > 0)
        {
            return string.Concat(key[..index].ToLowerInvariant(), key.AsSpan(index));
        }
        return key;
    }
}
