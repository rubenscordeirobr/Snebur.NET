namespace Snebur.UseCases.Abstractions.Identities;

public interface ITenantUserValidationService : IValidationService
{
    Task<bool> IsEmailUniqueAsync(
       string email,
       CancellationToken cancellationToken = default);

    Task<bool> IsEmailUniqueAsync(
        Guid currentUser_Id,
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> IsPhoneNumberUniqueAsync(
        string phoneNumber, 
        CancellationToken cancellationToken = default);

    Task<bool> IsPhoneNumberUniqueAsync(
        Guid currentUser_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default);
}
