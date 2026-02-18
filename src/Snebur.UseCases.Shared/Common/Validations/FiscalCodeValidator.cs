using Snebur.Core.Utils;

namespace Snebur.UseCases.Shared;
public static partial class DefaultValidationsExtensions
{
    public static IRuleBuilderOptions<T, FiscalCode> FiscalCodeCountry<T>(
        this IRuleBuilder<T, FiscalCode> ruleBuilder,
        IJsonStringLocalizer localizer,
        Country country)
    {
        Guard.NotNull(ruleBuilder);

        return ruleBuilder
            .SetValidator(new FiscalCodeValidator(country, localizer));

    }
    public static IRuleBuilderOptions<T, FiscalCode> FiscalCode<T>(
        this IRuleBuilder<T, FiscalCode> ruleBuilder,
        Func<T, Country> funcCountry,
        IJsonStringLocalizer localizer)
    {
        Guard.NotNull(ruleBuilder);

        return ruleBuilder
            .SetValidator((instance, fiscalCode) => new FiscalCodeValidator(funcCountry(instance), localizer));
    }
}

public class FiscalCodeValidator : AbstractValidator<FiscalCode>
{
    public FiscalCodeValidator(
        Country country,
        IJsonStringLocalizer localizer)
    {
        Guard.NotNull(localizer);

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(localizer["FiscalCode.Value", "Fiscal code is required."])
            .MaximumLength(ValidationConstants.FiscalCodeMaxLength)
            .WithMessage(localizer["FiscalCode.MaxLength", "Fiscal code cannot be longer than {MaxLength} characters."])
            .Must((fiscalCode, value) => FiscalCodeValidationUtils.IsValid(value, country))
            .WithMessage(localizer["FiscalCode.Invalid", "Fiscal code is Invalid for '{Country}' .", localizer[country]]);
    }
}
