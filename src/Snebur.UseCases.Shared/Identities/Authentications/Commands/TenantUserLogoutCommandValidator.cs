namespace Snebur.UseCases.Identities.Authentications.Commands;

public class TenantUserLogoutCommandValidator : CommandValidator<TenantUserLogoutCommand>
{
    public TenantUserLogoutCommandValidator(
        IJsonStringLocalizer<TenantUserLogoutCommand> localizer)
        : base(localizer)
    {
        Guard.NotNull(localizer);

        RuleFor(x => x.Session_Id)
            .NotEmptyGuid()
            .WithMessage(localizer["TenantUserLogout.Session_IdRequired", "Session Id is required."]);
    }
}
