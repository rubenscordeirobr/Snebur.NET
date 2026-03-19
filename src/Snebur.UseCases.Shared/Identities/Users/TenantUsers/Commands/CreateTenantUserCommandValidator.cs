namespace Snebur.UseCases.Identities.Users.TenantUsers.Commands;

public class CreateTenantUserCommandValidator : CommandValidator<CreateTenantUserCommand>
{
    private readonly ITenantUserValidationService _validationService;

    public CreateTenantUserCommandValidator(
        ITenantUserValidationService validationService,
        IJsonStringLocalizer<CreateTenantUserCommand> localizer)
        : base(localizer)
    {
        Guard.NotNull(validationService);
        Guard.NotNull(localizer);

        _validationService = validationService;

        RuleFor(x => x.Tenant_Id)
            .NotEmptyGuid()
            .WithMessage(localizer["Tenant.IdRequired", "Id is required."]);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(localizer["TenantUser.NameRequired", "Name is required."])
            .MinimumLength(ValidationConstants.NameMinLength)
            .WithMessage(localizer["TenantUser.NameTooShort", "Name cannot be shorter than {MinLength} characters."])
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage(localizer["TenantUser.NameTooLong", "Name cannot be longer than {MaxLength} characters."]);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(ValidationConstants.EmailMaxLength)
            .EmailAddressValid()
            .WithMessage(localizer["TenantUser.InvalidEmail", "Invalid email address."]);

        RuleFor(x => x.PhoneNumber)
            .PhoneNumber( localizer);

        RuleFor(x => x.Password)
            .CreatePassword(localizer);

        RuleFor(x => x.Role)
            .IsInEnumValue()
            .WithMessage(localizer["TenantUser.InvalidRole", "Invalid role."]);

        //Async validation
        RuleFor(x => x.Email)
            .MustAsync(IsEmailUniqueAsync)
            .WithMessage(localizer["TenantUser.EmailExists", "Email already exists."]);

        RuleFor(x => x.PhoneNumber)
            .MustAsync(IsPhoneNumberUniqueAsync)
            .WithMessage(localizer["TenantUser.PhoneNumberExists", "Phone number already exists."]);
    }

    private async Task<bool> IsEmailUniqueAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await _validationService.IsEmailUniqueAsync(email, cancellationToken);
    }

    private async Task<bool> IsPhoneNumberUniqueAsync(
        PhoneNumber phoneNumber,
        CancellationToken cancellationToken)
    {
        return await _validationService.IsPhoneNumberUniqueAsync(
            phoneNumber.FullNumber,
            cancellationToken);
    }
}
