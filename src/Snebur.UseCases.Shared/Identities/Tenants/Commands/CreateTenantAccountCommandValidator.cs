namespace Snebur.UseCases.Identities.Tenants.Commands;

public sealed class CreateTenantAccountCommandValidator : CommandValidator<CreateTenantAccountCommand>
{
    private readonly ITenantValidationService _tenantValidationService;

    public CreateTenantAccountCommandValidator(
        ITenantValidationService tenantValidationService,
        IJsonStringLocalizer<CreateTenantAccountCommand> localizer)
        : base(localizer)
    {
        Guard.NotNull(tenantValidationService);
        Guard.NotNull(localizer);

        _tenantValidationService = tenantValidationService;

        RuleFor(x=> x.Name)
            .NotEmpty()
            .WithMessage(localizer["Tenant.NameRequired", "Full Name is required."])
            .MinimumLength(ValidationConstants.NameMinLength)
            .WithMessage(localizer["Tenant.NameTooShort", "Name cannot be shorter than {MinLength} characters."])
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage(localizer["Tenant.NameTooLong", "Name cannot be longer than {MaxLength} characters."])  
            .FullName()
            .WithMessage(localizer["Tenant.InvalidFullname", "Name must contain first name and last name."]);

        RuleFor(x => x.BusinessName)
            .NotEmpty()
            .WithMessage(localizer["Tenant.BusinessNameRequired", "Business name is required."])
            .MinimumLength(ValidationConstants.NameMinLength)
            .WithMessage(localizer["Tenant.TenantNameTooShort", "Tenant name cannot be shorter than {MinLength} characters."])
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage(localizer["Tenant.TenantNameTooLong", "Tenant name cannot be longer than {MaxLength} characters."]);
         
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(ValidationConstants.EmailMaxLength)
            .EmailAddressValid()
            .WithMessage(localizer["Tenant.InvalidEmail", "Invalid email address."]);

        RuleFor(x => x.PhoneNumber)
            .NotNull()
            .WithMessage(localizer["Tenant.PhoneNumberRequired", "Phone number is required."])
            .PhoneNumber(localizer);

        RuleFor(x => x.Password)
            .CreatePassword(localizer);

        RuleFor(x => x.Country)
            .IsInEnumValue()
            .WithMessage(localizer["Tenant.InvalidCountry", "Invalid country."]);

        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage(localizer["Tenant.CultureRequired", "Culture is required."])
            .IsInEnumValue()
            .WithMessage(localizer["Tenant.InvalidCulture", "Invalid culture."]);

        RuleFor(x => x.Currency)
            .IsInEnumValue()
            .WithMessage(localizer["Tenant.InvalidCurrency", "Invalid currency."]);

        RuleFor(x => x.BusinessType)
            .IsInEnumValue()
            .WithMessage(localizer["Tenant.InvalidBusinessType", "Invalid business type."]);

        RuleFor(x => x.TenantType)
            .IsInEnumValue()
            .WithMessage(localizer["Tenant.InvalidTenantType", "Invalid tenant type."]);

        RuleFor(x => x.TenantType)
            .IsInEnumValue()
            .WithMessage(localizer["Tenant.InvalidTenantType", "Invalid tenant type."]);

        RuleFor(x => x.FiscalCode)
            .FiscalCode(x => x.Country, localizer);

        //Async validation
        RuleFor(x => x.PhoneNumber)
            .MustAsync(IsPhoneNumberAsync)
            .WithErrorCode("Tenant.PhoneNumberUniqueValidation")
            .WithMessage(
                localizer["Tenant.PhoneNumberUniqueValidation", "The phone number '{PropertyValue} is already in use."]);

        RuleFor(x => x.Email)
            .MustAsync(IsEmailUniqueAsync)
            .WithErrorCode("Tenant.EmailUniqueValidation")
            .WithMessage(
                localizer["Tenant.EmailUniqueValidation", "The e-mail '{PropertyValue} is already in use."]);

        RuleFor(x => x.FiscalCode)
            .MustAsync(IsFiscalCodeUniqueAsync)
            .WithErrorCode("Tenant.FiscalCodeUniqueValidation")
            .WithMessage(
                localizer["Tenant.FiscalCodeUniqueValidation",
                          "The fiscal code '{PropertyValue}' is already in use."]);
    }

    private async Task<bool> IsPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken token)
    {
        return await _tenantValidationService.IsPhoneNumberUniqueAsync(phoneNumber.FullNumber, token);
    }

    private async Task<bool> IsEmailUniqueAsync(string email, CancellationToken token)
    {
        return await _tenantValidationService.IsEmailUniqueAsync(email, token);
    }

    private async Task<bool> IsFiscalCodeUniqueAsync(FiscalCode fiscalCode, CancellationToken token)
    {
        return await _tenantValidationService.IsFiscalCodeUniqueAsync(fiscalCode.Value, token);
    }
}
