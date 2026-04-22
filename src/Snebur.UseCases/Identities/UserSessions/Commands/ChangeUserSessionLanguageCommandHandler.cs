namespace Snebur.UseCases.Identities.UserSessions.Commands;

public class ChangeUserSessionLanguageCommandHandler 
    : CommandHandler<ChangeUserSessionLanguageCommand, OperationResponse>
{
    private readonly IIdentityUnitOfWork _unitOfWork;
    public ChangeUserSessionLanguageCommandHandler(
        IIdentityUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    protected override async Task<Result<OperationResponse>> HandleAsync(
        ChangeUserSessionLanguageCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var userSession = await _unitOfWork.UserSessions
            .GetByIdAsync(command.UserSession_Id, cancellationToken);

        if (userSession is null)
        {
            return Result.NotFoundFailure<OperationResponse>(
                "UserSession.NotFound",
                $"User session with id {command.UserSession_Id} not found.");
        }

        userSession.ChangeLanguage(command.Language);

        var result = await _unitOfWork.SaveChangesAsync(silent: true, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<OperationResponse>(result.Error);
        }
        return Result.Success(new OperationResponse());
    }
}
