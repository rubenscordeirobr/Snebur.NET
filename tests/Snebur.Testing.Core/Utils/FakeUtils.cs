using Snebur.Core.Utils;

namespace Snebur.Testing.Core.Utils;

public static class FakeUtils
{
    public static string GenerateFakeEmail()
    {
        return $"fake{RandomUtils.GenerateRandomNumber(10)}@example.com";
    }
}
