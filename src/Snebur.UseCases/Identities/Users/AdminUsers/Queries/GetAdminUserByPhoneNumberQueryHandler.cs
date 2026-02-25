using Snebur.UseCases.Mappers.Identities;

namespace Snebur.UseCases.Identities.Users.AdminUsers.Queries;

public class GetAdminUserByPhoneNumberQueryHandler
    : IGetQueryResultHandler<GetAdminUserByPhoneNumberQuery, UserResponse>
{
    private readonly IAdminUserRepository _adminUserRepository;
    public GetAdminUserByPhoneNumberQueryHandler(
        IAdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;
    }

    public async Task<Result<UserResponse>> HandleAsync(
        GetAdminUserByPhoneNumberQuery query,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(query);

        var user = await _adminUserRepository.GetByPhoneNumberAsync(query.PhoneNumber, cancellationToken);
        if (user is null)
        {
            return Result.NotFoundFailure<UserResponse>(
                "SystemUser.NotFound",
                $"SystemUser with phone number {query.PhoneNumber} not found.");
        }

        var userResponse = UserMapper.ToResponse(user);
        return Result.Success(userResponse);
    }
}
