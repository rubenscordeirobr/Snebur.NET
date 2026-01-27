using System.Security.Cryptography;

namespace Snebur.Core.Helpers;
public static class PasswordHelper
{
    private static bool ContainsUpperCase(string input)
        => input.Any(char.IsUpper);
   
    private static bool ContainsNumber(string input)
        => input.Any(char.IsDigit);
 
    private static bool ContainsSpecialChar(string input)
        => input.Any(ch => !char.IsLetterOrDigit(ch));

    public static PasswordStrength CalculateStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return PasswordStrength.Empty;

        var score = 0;
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;

        if (ContainsUpperCase(password)) score++;
        if (ContainsNumber(password)) score++;
        if (ContainsSpecialChar(password)) score++;

        return score switch
        {
            < 3 => PasswordStrength.Weak,
            3 => PasswordStrength.Medium,
            _ => PasswordStrength.Strong,
        };
    }

    public static string HashPassword(string password, string salt)
    {
        var passwordCombined = $"{password}::{salt}";
        return HashHelper.CreateSha256Hash(passwordCombined);
    }
     
    public static bool VerifyPassword(
        string inputPassword, 
        string passwordHash, 
        string salt)
    {
        var inputPasswordHash = HashPassword(inputPassword, salt);
        return passwordHash == inputPasswordHash;
    }

    public static string GenerateRandomPassword(int length = 12)
    {
        if (length < 4)
            throw new ArgumentException("Password length must be at least 4");

        // Define allowed character sets
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()-_=+[]{};:,.<>?";

        var passwordChars = new char[length];
         
        passwordChars[0] = uppercase[RandomNumberGenerator.GetInt32(0, uppercase.Length)];
        passwordChars[1] = lowercase[RandomNumberGenerator.GetInt32(0, lowercase.Length)];
        passwordChars[2] = digits[RandomNumberGenerator.GetInt32(0, digits.Length)];
        passwordChars[3] = special[RandomNumberGenerator.GetInt32(0, special.Length)];

        string allChars = uppercase + lowercase + digits + special;
        for (int i = 4; i < length; i++)
        {
            passwordChars[i] = allChars[RandomNumberGenerator.GetInt32(0, allChars.Length)];
        }

        Shuffle(passwordChars);

        return new string(passwordChars);
    }
    private static void Shuffle(char[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
