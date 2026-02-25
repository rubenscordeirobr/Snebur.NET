using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.TenantUsers.Queries;

public class GetTenantUserByEmailOrPhoneNumberQueryHandler
    : IGetQueryResultHandler<GetTenantUserByEmailOrPhoneNumberQuery, UserResponse>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    public GetTenantUserByEmailOrPhoneNumberQueryHandler(
        ITenantUserRepository tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }
    public async Task<Result<UserResponse>> HandleAsync(
        GetTenantUserByEmailOrPhoneNumberQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _tenantUserRepository.GetByEmailOrPhoneNumberAsync(query.EmailOrPhoneNumber, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "TenantUser.NotFound",
                $"TenantUser with email or phone number {query.EmailOrPhoneNumber} not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}
