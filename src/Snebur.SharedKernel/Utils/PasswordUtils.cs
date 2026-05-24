using Snebur.SharedKernel.Constants;

namespace Snebur.SharedKernel.Utils;

public static class PasswordValidationUtils
{
    public static bool IsCreatePasswordValid(string password)
    {
        return !string.IsNullOrWhiteSpace(password) &&
               password.Length >= ValidationConstants.PasswordMinLength &&
               password.Length <= ValidationConstants.PasswordMaxLength &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(c => !char.IsLetterOrDigit(c));
    }
}
