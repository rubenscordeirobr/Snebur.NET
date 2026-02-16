using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.SystemUsers.Queries;

public sealed class GetSystemUserByIdQueryHandler
    : IGetQueryResultHandler<GetSystemUserByIdQuery, UserResponse>
{
    private readonly ISystemUserRepository _systemUserRepository;
    public GetSystemUserByIdQueryHandler(
        ISystemUserRepository systemUserRepository)
    {
        _systemUserRepository = systemUserRepository;
    }

    public async Task<Result<UserResponse>> HandleAsync(
        GetSystemUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _systemUserRepository.GetByIdAsync(query.Id, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "SystemUser.NotFound",
                $"SystemUser with id {query.Id} not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}
