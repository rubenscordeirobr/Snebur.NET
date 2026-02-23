using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.TenantUsers.Queries;

public class GetTenantUserByIdQueryHandler
    : IGetQueryResultHandler<GetTenantUserByIdQuery, UserResponse>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    public GetTenantUserByIdQueryHandler(
        ITenantUserRepository tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<Result<UserResponse>> HandleAsync(
        GetTenantUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _tenantUserRepository.GetByIdAsync(query.Id, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "TenantUser.NotFound",
                $"TenantUser with id {query.Id} not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}

