using Snebur.Core.Utils;
using FluentValidation.Validators;

namespace Snebur.UseCases.Common.Validations;

public static partial class DefaultValidationsExtensions
{
    public static IRuleBuilderOptions<T, string> EmailAddressOrPhoneNumber<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        Guard.NotNull(ruleBuilder);

        return ruleBuilder.SetValidator(new EmailValidValidator<T>());
    }
}

public class EmailAddressOrPhoneNumberValidator<T> : PropertyValidator<T, string>, IEmailValidator
{
    public override string Name
        => "EmailAddressOrPhoneNumberValidator";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return ValidationUtils.IsEmail(value) ||
            ValidationUtils.IsFullPhoneNumberValid(value);
    }
}

