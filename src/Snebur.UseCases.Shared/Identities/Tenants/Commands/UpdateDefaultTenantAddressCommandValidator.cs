namespace Snebur.UseCases.Identities.Tenants.Commands;

public class UpdateDefaultTenantAddressCommandValidator : AbstractValidator<UpdateDefaultTenantAddressCommand>
{
    public UpdateDefaultTenantAddressCommandValidator(
        IJsonStringLocalizer<UpdateDefaultTenantAddressCommand> localizer,
        IJsonStringLocalizer<AddressValidator> addressLocalizer)
    {
        Guard.NotNull(localizer);

        RuleFor(x => x.Tenant_Id)
            .NotEmpty()
            .WithMessage(localizer["Tenant.IdRequired", "Tenant Id is required."]);

        RuleFor(x => x.AddressName)
            .NotEmpty()
            .WithMessage(localizer["Tenant.AddressNameRequired", "Address name is required."])
            .MaximumLength(ValidationConstants.AddressNameMaxLength)
            .WithMessage(localizer["Tenant.AddressNameTooLong", "Address name cannot be longer than {MaxLength} characters."]);

        RuleFor(x => x.Address)
            .NotNull()
            .SetValidator(new AddressValidator(addressLocalizer));
    }
}
