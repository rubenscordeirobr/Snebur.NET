using Snebur.Core.Utils;

namespace Snebur.Persistence.Identity.Repositories;

internal abstract class UserRepository<TUserEntity> : RepositoryBase<TUserEntity>, IUserRepository<TUserEntity>
    where TUserEntity : User
{
    protected override int DefaultMaxRecords
        => GetDefaultMaxRecords();

  

    protected UserRepository(IdentityDbContext dbContext,
        IHttpContextSessionAccessor userSessionAccessor,
        TrackingOption trackingOption = TrackingOption.NoTracking)
        : base(dbContext, userSessionAccessor, trackingOption)
    {
    }

    public Task<TUserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => FindAsync(x => x.Email == email, cancellationToken);

    public Task<TUserEntity?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => FindAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);

    public Task<TUserEntity?> GetByEmailOrPhoneNumberAsync(
        string emailOrPhoneNumber, 
        CancellationToken cancellationToken = default)
    {
        if (ValidationUtils.IsEmail(emailOrPhoneNumber))
        {
            return GetByEmailAsync(emailOrPhoneNumber, cancellationToken);
        }
        return GetByPhoneNumberAsync(emailOrPhoneNumber, cancellationToken);
    }
    #region Validations

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
       => AnyAsync(x => x.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, Guid user_Id, CancellationToken cancellationToken = default)
        => AnyAsync(x => x.Email == email && x.Id != user_Id, cancellationToken);

    public Task<bool> PhoneNumberExistsAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => AnyAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);

    public Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid user_Id, CancellationToken cancellationToken = default)
        => AnyAsync(x => x.PhoneNumber == phoneNumber && x.Id != user_Id, cancellationToken);

    public Task<bool> EmailOrPhoneNumberExistsAsync(
        string emailOrPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        if (ValidationUtils.IsEmail(emailOrPhoneNumber))
        {
            return EmailExistsAsync(emailOrPhoneNumber, cancellationToken);
        }
        return PhoneNumberExistsAsync(emailOrPhoneNumber, cancellationToken);
    }

    public Task<bool> EmailOrPhoneNumberExistsAsync(
        string emailOrPhoneNumber,
        Guid user_Id,
        CancellationToken cancellationToken = default)
    {
        if (ValidationUtils.IsEmail(emailOrPhoneNumber))
        {
            return EmailExistsAsync(emailOrPhoneNumber, user_Id, cancellationToken);
        }
        return PhoneNumberExistsAsync(emailOrPhoneNumber, user_Id, cancellationToken);
    }

    #endregion

    private int GetDefaultMaxRecords()
    {
        return (!UserSession.IsTenantUser() && !UserSession.IsSystemAdminUser())
            ? 1
            : base.DefaultMaxRecords;
    }
}

