namespace Snebur.UseCases.Shared;

public static partial class DefaultValidationsExtensions
{
    public static IRuleBuilderOptions<T, Password> Password<T>(
     this IRuleBuilder<T, Password> ruleBuilder,
     IJsonStringLocalizer localizer)
    {
        Guard.NotNull(ruleBuilder);

        return ruleBuilder
            .SetValidator(new PasswordValidator(localizer));
    }

    public static IRuleBuilderOptions<T, string> CreatePassword<T>(
         this IRuleBuilder<T, string> ruleBuilder,
         IJsonStringLocalizer localizer)
    {
        Guard.NotNull(ruleBuilder);
        Guard.NotNull(localizer);

        var options = ruleBuilder
            .NotEmpty()
                .WithMessage(localizer["PasswordEmpty", "Password cannot be empty"])
            .MinimumLength(ValidationConstants.PasswordMinLength)
                .WithMessage(localizer["PasswordTooShort", "Password must be at least {MinLength} characters long"])
            .MaximumLength(ValidationConstants.PasswordMaxLength)
                .WithMessage(localizer["PasswordTooLong", "Password cannot be longer than {MaxLength} characters"])
            .Matches("[A-Z]")
                .WithMessage(localizer["PasswordUppercase", "Password must contain at least one uppercase letter"])
            .Matches("[a-z]")
                .WithMessage(localizer["PasswordLowercase", "Password must contain at least one lowercase letter"])
            .Matches("[0-9]")
                .WithMessage(localizer["PasswordNumber", "Password must contain at least one number"])
            .Matches("[^a-zA-Z0-9]")
                .WithMessage(localizer["PasswordSpecialCharacter", "Password must contain at least one special character"]);

        return options;
    }
}

public class PasswordValidator : AbstractValidator<Password>
{
    public PasswordValidator(IJsonStringLocalizer localizer)
    {
        Guard.NotNull(localizer);

        RuleFor(x => x.Strength)
            .NotEqual(PasswordStrength.Empty)
            .WithMessage(localizer["PasswordStrength.Empty", "Password strength cannot be empty."])
            .NotEqual(PasswordStrength.Weak)
            .WithMessage(localizer["PasswordStrength.Weak", "Password strength cannot be weak."]);

        RuleFor(x => x.HashValue)
            .NotEmpty()
            .WithMessage(localizer["PasswordHashValueEmpty", "Password hash value cannot be empty."])
            .Length(ValidationConstants.PasswordHashLength)
            .Sha256()
            .WithMessage(localizer["PasswordHashValueInvalid", "Password hash value is invalid."]);
    }
}
