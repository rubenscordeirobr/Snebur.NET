using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.TenantUsers.Queries;

public class GetTenantUserByEmailQueryHandler
    : IGetQueryResultHandler<GetTenantUserByEmailQuery, UserResponse>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    public GetTenantUserByEmailQueryHandler(
        ITenantUserRepository tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<Result<UserResponse>> HandleAsync(
        GetTenantUserByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _tenantUserRepository.GetByEmailAsync(query.Email, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "TenantUser.NotFound",
                $"TenantUser with email {query.Email} not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}
