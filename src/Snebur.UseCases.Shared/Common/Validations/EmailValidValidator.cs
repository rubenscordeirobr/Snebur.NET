using Snebur.Core.Exceptions;
using Snebur.Core.Utils;
using FluentValidation.Validators;

namespace Snebur.UseCases.Shared;

public static partial class DefaultValidationsExtensions
{
    [Obsolete("Use EmailAddressValid ")]
    public static IRuleBuilderOptions<T, string> EmailAddress<T>(
        this IRuleBuilder<T, string> ruleBuilder, 
        EmailValidationMode mode = EmailValidationMode.AspNetCoreCompatible)
    {
        throw new DeprecatedException($"Use {nameof(EmailAddressValid)}");
    }

    public static IRuleBuilderOptions<T, string> EmailAddressValid<T>(
     this IRuleBuilder<T, string> ruleBuilder)
    {
        Guard.NotNull(ruleBuilder);

        return ruleBuilder.SetValidator(new EmailValidValidator<T>());
    }
}

public class EmailValidValidator<T> : PropertyValidator<T, string>, IEmailValidator
{
    public override string Name
        => "EmailValidValidator";
 
    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return ValidationUtils.IsEmail(value);
    }
}
