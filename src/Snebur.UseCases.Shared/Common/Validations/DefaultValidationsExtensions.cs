using System.Text.RegularExpressions;

namespace Snebur.UseCases.Common.Validations;

public static partial class DefaultValidationsExtensions
{
    private static readonly Regex FullNameRegex = new(@"^\p{L}[\p{L}\d'’.\-\s_]*\s+\p{L}[\p{L}\d'’.\-\s_]*$", RegexOptions.Compiled);
     
    public static IRuleBuilderOptions<T, string> FullName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .Matches(FullNameRegex);
    }

    //NotEmptyGuid
    public static IRuleBuilderOptions<T, Guid> NotEmptyGuid<T>(
        this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(Guid.Empty);
    }

    //Sha256
    public static IRuleBuilderOptions<T, string> Sha256<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Matches("^[a-f0-9]{64}$");
    }
}
