namespace Snebur.Core.Utils;

public static class RandomUtils
{
    private static readonly Random _random = new();

    
    public static string GenerateRandomNumber(int length)
    {
        var digits = new char[length];
        for (int i = 0; i < length; i++)
        {
            digits[i] = (char)('0' + _random.Next(0, 10));
        }
        return new string(digits);
    }
}
