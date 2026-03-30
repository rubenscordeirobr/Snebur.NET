namespace Snebur.UseCases.Identities.Authentications.Commands;

public class AdminUserLoginCommandValidator : CommandValidator<AdminUserLoginCommand>
{
    private readonly IAdminUserAuthenticationValidationService _validationService;

    public AdminUserLoginCommandValidator(
        IAdminUserAuthenticationValidationService validationService,
        IJsonStringLocalizer<AdminUserLoginCommand> localizer)
        : base(localizer)
    {
        Guard.NotNull(validationService);
        Guard.NotNull(localizer);

        _validationService = validationService;

        RuleFor(x => x.EmailOrPhoneNumber).
            NotEmpty()
                .WithMessage(localizer["AdminAuthentication.EmailOrPhoneNumberRequired", "Email or phone number is required."])
           .MaximumLength(ValidationConstants.EmailMaxLength)
                .WithMessage(localizer["AdminAuthentication.EmailOrPhoneNumberMaxLength", $"Email or phone number must be less than {ValidationConstants.EmailMaxLength} characters."])
           .EmailAddressOrPhoneNumber()
                .WithMessage(localizer["AdminAuthentication.InvalidEmailOrPhoneNumber", "Invalid email or phone number."]);

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(localizer["AdminAuthentication.PasswordRequired", "Password is required."])
            .MinimumLength(ValidationConstants.PasswordMinLength)
                .WithMessage(localizer["AdminAuthentication.PasswordTooShort", "Password must be at least {MinLength} characters long"])
            .MaximumLength(ValidationConstants.PasswordMaxLength)
                .WithMessage(localizer["AdminAuthentication.PasswordTooLong", "Password cannot be longer than {MaxLength} characters"]);

        //Async
        RuleFor(x => x.EmailOrPhoneNumber)
            .MustAsync(IsEmailOrPhoneNumberExitsAsync)
                .WithMessage(localizer["AdminAuthentication.EmailOrPhoneNumberNotExists", "Email or phone number does not exist."]);
    }
     
    private Task<bool> IsEmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken)
    {
        return _validationService.EmailOrPhoneNumberExitsAsync(
            emailOrPhoneNumber,
            cancellationToken);
    }
}

