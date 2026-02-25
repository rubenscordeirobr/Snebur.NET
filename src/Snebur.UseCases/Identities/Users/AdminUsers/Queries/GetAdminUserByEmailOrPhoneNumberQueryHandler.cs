
using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public class GetAdminUserByEmailOrPhoneNumberQueryHandler
    : IGetQueryResultHandler<GetAdminUserByEmailOrPhoneNumberQuery, UserResponse>
{
    private readonly IAdminUserRepository _adminUserRepository;
    public GetAdminUserByEmailOrPhoneNumberQueryHandler(
        IAdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;
    }
    public async Task<Result<UserResponse>> HandleAsync(
        GetAdminUserByEmailOrPhoneNumberQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _adminUserRepository.GetByEmailOrPhoneNumberAsync(query.EmailOrPhoneNumber, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "SystemUser.NotFound",
                $"SystemUser with email or phone number  {query.EmailOrPhoneNumber}  not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}
