namespace Snebur.UseCases.Identities.Authentications.Commands;

public class TenantUserLoginCommandValidator : CommandValidator<TenantUserLoginCommand>
{
    private readonly ITenantUserAuthenticationValidationService _validationService;

    public TenantUserLoginCommandValidator(
        ITenantUserAuthenticationValidationService validationService,
        IJsonStringLocalizer<TenantUserLoginCommand> localizer)
        : base(localizer)
    {
        Guard.NotNull(validationService);
        Guard.NotNull(localizer);

        _validationService = validationService;

        RuleFor(x => x.EmailOrPhoneNumber).
            NotEmpty()
                .WithMessage(localizer["TenantUserAuthentication.EmailOrPhoneNumberRequired", "Email or phone number is required."])
           .MaximumLength(ValidationConstants.EmailMaxLength)
                .WithMessage(localizer["TenantUserAuthentication.EmailOrPhoneNumberMaxLength", $"Email or phone number must be less than {ValidationConstants.EmailMaxLength} characters."])
           .EmailAddressOrPhoneNumber()
                .WithMessage(localizer["TenantUserAuthentication.InvalidEmailOrPhoneNumber", "Invalid email or phone number."]);

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(localizer["TenantUserAuthentication.PasswordRequired", "Password is required."])
            .MinimumLength(ValidationConstants.PasswordMinLength)
                .WithMessage(localizer["TenantUserAuthentication.PasswordTooShort", "Password must be at least {MinLength} characters long"])
            .MaximumLength(ValidationConstants.PasswordMaxLength)
                .WithMessage(localizer["TenantUserAuthentication.PasswordTooLong", "Password cannot be longer than {MaxLength} characters"]);

        //Async
        RuleFor(x => x.EmailOrPhoneNumber)
            .MustAsync(IsEmailOrPhoneNumberExitsAsync)
                .WithMessage(localizer["TenantUserAuthentication.EmailOrPhoneNumberNotExists", "Email or phone number does not exist."]);
    }
      
    private Task<bool> IsEmailOrPhoneNumberExitsAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken)
    {
        return _validationService.EmailOrPhoneNumberExitsAsync(
            emailOrPhoneNumber,
            cancellationToken);
    }

    private Task<bool> VerifyTenantUserCredentialsAsync(
     TenantUserLoginCommand command,
     CancellationToken cancellationToken)
    {
        return _validationService
            .VerifyTenantUserCredentialsAsync(
                command.EmailOrPhoneNumber,
                command.Password,
                cancellationToken);
    }
}

