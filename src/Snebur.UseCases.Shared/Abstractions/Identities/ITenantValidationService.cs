namespace Snebur.UseCases.Abstractions.Identities;

public interface ITenantValidationService: IValidationService
{
    Task<bool> IsEmailUniqueAsync(
        string email, 
        CancellationToken cancellationToken = default);
   
    Task<bool> IsEmailUniqueAsync(
        Guid currentTenant_Id,
        Guid currentTenantOwner_Id,
        string email, 
        CancellationToken cancellationToken = default);

    Task<bool> IsFiscalCodeUniqueAsync(
        string fiscalCode, 
        CancellationToken cancellationToken = default);
  
    Task<bool> IsFiscalCodeUniqueAsync(
        Guid currentTenant_Id,
        string fiscalCode,
        CancellationToken cancellationToken = default);

    Task<bool> IsPhoneNumberUniqueAsync(
        string phoneNumber, 
        CancellationToken cancellationToken = default);
   
    Task<bool> IsPhoneNumberUniqueAsync(
        Guid currentTenant_Id,
        Guid currentTenantOwner_Id,
        string phoneNumber,
        CancellationToken cancellationToken = default);
}
