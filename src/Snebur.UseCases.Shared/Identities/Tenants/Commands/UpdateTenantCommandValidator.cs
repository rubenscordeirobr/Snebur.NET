 

namespace Snebur.UseCases.Identities.Tenants.Commands;

public class UpdateTenantCommandValidator : CommandValidator<UpdateTenantCommand>
{
    private readonly ITenantValidationService _tenantValidationService;

    public UpdateTenantCommandValidator(
        ITenantValidationService tenantValidationService,
        IJsonStringLocalizer<UpdateTenantCommand> localizer) 
        : base(localizer)
    {

        Guard.NotNull(tenantValidationService);
        Guard.NotNull(localizer);

        _tenantValidationService = tenantValidationService;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(localizer["Tenant.NameRequired", "Name is required."])
            .MinimumLength(ValidationConstants.NameMinLength)
            .WithMessage(localizer["Tenant.NameTooShort", "Name cannot be shorter than {MinLength} characters."])
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage(localizer["Tenant.NameTooLong", "Name cannot be longer than {MaxLength} characters."])
            .FullName()
            .WithMessage(localizer["Tenant.InvalidFullname", "Name must contain first name and last name."]);
 
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

        RuleFor(x => x.FiscalCode)
            .FiscalCode(x => x.Country, localizer);

        RuleFor(x => x.FiscalCode)
         .MustAsync(IsFiscalCodeUniqueAsync)
         .WithErrorCode("Tenant.FiscalCodeUniqueValidation")
         .WithMessage(
             localizer["Tenant.FiscalCodeUniqueValidation",
                       "The fiscal code '{PropertyValue}' is already in use."]);

    }
    private async Task<bool> IsFiscalCodeUniqueAsync(
        UpdateTenantCommand command, 
        FiscalCode fiscalCode,
        CancellationToken token)
    {
        return await _tenantValidationService.IsFiscalCodeUniqueAsync(command.Tenant_Id, fiscalCode.Value, token);
    }
}
