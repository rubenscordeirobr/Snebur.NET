namespace Snebur.UseCases.Identities.Authentications.Commands;

public record AdminUserLogoutCommand(
    Guid Session_Id)
    : CommandRequest<OperationResponse>;

