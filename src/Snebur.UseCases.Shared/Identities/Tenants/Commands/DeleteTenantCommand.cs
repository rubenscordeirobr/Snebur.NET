namespace Snebur.UseCases.Identities.Tenants.Commands;

public record DeleteTenantCommand(Guid Tenant_Id) : CommandRequest<OperationResponse>;
public class DeleteTenantCommandValidator : CommandValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator(
        IJsonStringLocalizer<DeleteTenantCommand> localizer)
        : base(localizer)
    {
        Guard.NotNull(localizer);

        RuleFor(x => x.Tenant_Id)
            .NotEmptyGuid()
            .WithMessage(localizer["Tenant.IdRequired", "Id is required."]);
    }
}
