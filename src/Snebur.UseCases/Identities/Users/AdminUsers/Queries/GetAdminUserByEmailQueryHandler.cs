using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public class GetAdminUserByEmailQueryHandler
    : IGetQueryResultHandler<GetAdminUserByEmailQuery, UserResponse>
{
    private readonly IAdminUserRepository _adminUserRepository;

    public GetAdminUserByEmailQueryHandler(
        IAdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;
    }

    public async Task<Result<UserResponse>> HandleAsync(
        GetAdminUserByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _adminUserRepository.GetByEmailAsync(query.Email, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "SystemUser.NotFound",
                $"SystemUser with email {query.Email} not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}
