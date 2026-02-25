using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.TenantUsers.Queries;

public class GetTenantUserByPhoneNumberQueryHandler
    : IGetQueryResultHandler<GetTenantUserByPhoneNumberQuery, UserResponse>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    public GetTenantUserByPhoneNumberQueryHandler(
        ITenantUserRepository tenantUserRepository)
    {
        Guard.NotNull(tenantUserRepository);

        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<Result<UserResponse>> HandleAsync(
        GetTenantUserByPhoneNumberQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _tenantUserRepository.GetByPhoneNumberAsync(query.PhoneNumber, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "TenantUser.NotFound",
                $"TenantUser with phone number {query.PhoneNumber} not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}
