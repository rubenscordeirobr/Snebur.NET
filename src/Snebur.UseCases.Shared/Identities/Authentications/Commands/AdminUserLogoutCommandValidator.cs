namespace Snebur.UseCases.Identities.Authentications.Commands;

public class AdminUserLogoutCommandValidator : CommandValidator<AdminUserLogoutCommand>
{
    public AdminUserLogoutCommandValidator(
        IJsonStringLocalizer<AdminUserLogoutCommand> localizer)
        : base(localizer)
    {
        Guard.NotNull(localizer);

        RuleFor(x => x.Session_Id)
            .NotEmptyGuid()
            .WithMessage(localizer["AdminUserLogout.Session_IdRequired", "Session Id is required."]);
    }
}
