
namespace Snebur.ArchitectureTests.Extensions;

public static class TestResultExtensions
{
    public static string GetFailingTypeNames(this NetArchTest.Rules.TestResult result)
    {
        if (result.FailingTypeNames is null)
            return string.Empty;

        return string.Join(",", result.FailingTypeNames);
    }
}
