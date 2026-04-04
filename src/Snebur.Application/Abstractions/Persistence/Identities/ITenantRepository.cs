namespace Snebur.Application.Abstractions.Persistence.Identities;

public interface ITenantRepository : IRepositoryBase<Tenant>
{
    Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken);

    #region Validations
    Task<bool> EmailExistsAsync(string email, CancellationToken token);
    Task<bool> EmailExistsAsync( string email, Guid currentTenant_Id, CancellationToken token);
    Task<bool> FiscalCodeExistsAsync( string fiscalCode, CancellationToken token);
    Task<bool> FiscalCodeExistsAsync( string fiscalCode, Guid currentTenant_Id, CancellationToken token);
    Task<bool> PhoneNumberExitsAsync(string phoneNumber, CancellationToken token);
    Task<bool> PhoneNumberExitsAsync(string phoneNumber, Guid currentTenant_Id, CancellationToken token);

    #endregion
}
