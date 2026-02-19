using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public class GetAdminUserByIdQueryHandler
     : IGetQueryResultHandler<GetAdminUserByIdQuery, UserResponse>
{
    private readonly IAdminUserRepository _adminUserRepository;
    public GetAdminUserByIdQueryHandler(
        IAdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;

    }
    public async Task<Result<UserResponse>> HandleAsync(
        GetAdminUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _adminUserRepository.GetByIdAsync(query.Id, cancellationToken);
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

