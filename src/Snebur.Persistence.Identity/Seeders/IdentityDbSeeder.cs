using Snebur.Application.Abstractions.Security;
using Snebur.Core.Infos;
using Snebur.Domain.Entities.Identities.Factories;

namespace Snebur.Persistence.Identity.Seeders;

internal class IdentityDbSeeder
{
    private readonly IdentityDbContext _dbContext;
    private readonly ISecureConfiguration _secureConfiguration;

    internal IdentityDbSeeder(
        IdentityDbContext dbContext,
        ISecureConfiguration secureConfiguration)
    {
        _dbContext = dbContext;
        _secureConfiguration = secureConfiguration;
    }
     
    public async Task SeedAsync()
    {
        AddAnonymousUserAndSessionAsync();
        AddDefaultAdminUser();
        AddSystemTenant();

        if (_dbContext is not IDbSeedAsync _)
        {
            throw new InvalidOperationException("DbContext must implement IDbSeedAsync");
        }

        var result = await (_dbContext as IDbSeedAsync).SeedSaveChangesAsync();
        if (result < 1)
        {
            throw new InvalidOperationException("Seed failed");
        }
    }

    private void AddAnonymousUserAndSessionAsync()
    {
        var salt = _secureConfiguration.GetPasswordSalt();
        var strongPassword = "%ANONYMOUS@anonymous%";
        var phoneNumber = PhoneNumber.Create("+5542999999999").GetRequiredValue();
        var password = Password.Create(strongPassword, salt).GetRequiredValue();

        var anonymousUser = new SystemUser(
            name: "Anonymous",
            email: "anonymous@snebur.com.br",
            language: Language.Default,
            role: UserRole.Anonymous,
            userState: UserState.Active,
            userStatus: UserStatus.Anonymous,
            phoneNumber: phoneNumber,
            password);

        anonymousUser.SetAnonymousId();

        var headerInfo = ClientRequestHeaderInfo.System;

        var anonymousUserSession = UserSessionFactory.Create(
            user: anonymousUser,
            clientHeaderInfo: headerInfo,
            authenticationType: AuthenticationType.Anonymous,
            isPersistent: true,
            tenant_id: null);

        anonymousUserSession.SetAnonymousSystemSessionId();

        _dbContext.Add(anonymousUser);
        _dbContext.Add(anonymousUserSession);
    }

    private void AddDefaultAdminUser()
    {
        var salt = _secureConfiguration.GetPasswordSalt();
        var strongPassword = DefaultAdminUserConstants.TestPassword;
        var phoneNumber = PhoneNumber.Create(DefaultAdminUserConstants.PhoneNumber).GetRequiredValue();

        var password = Password.Create(strongPassword, salt).GetRequiredValue();

        var adminUser = new AdminUser(
            name: DefaultAdminUserConstants.Name,
            email: DefaultAdminUserConstants.Email,
            language: Language.Default,
            role: UserRole.Admin,
            userState: UserState.Active,
            userStatus: UserStatus.System,
            phoneNumber: phoneNumber,
            password);

        adminUser.SetCreateSession(AnonymousUserConstants.Session_Id);
        adminUser.SetPropertyValue(x => x.Id, DefaultAdminUserConstants.User_Id);

        _dbContext.Add(adminUser);
    }

    internal void AddSystemTenant()
    {
        var salt = _secureConfiguration.GetPasswordSalt();
        var testPassword = SystemTenantConstants.TestPassword;

        var password = Password.Create(testPassword, salt)
            .GetRequiredValue();

        var phoneNumber = PhoneNumber.Create(SystemTenantConstants.PhoneNumber)
            .GetRequiredValue();

        var fiscalCode = FiscalCode.Create(SystemTenantConstants.FiscalCode, Country.Brazil).Value!;

        var systemTenant = new Tenant(
            name: SystemTenantConstants.Name,
            email: SystemTenantConstants.Email,
            businessType: BusinessType.System,
            country: Country.Brazil,
            currency: Currency.BRL,
            language: Language.Default,
            tenantState: TenantState.System,
            tenantStatus: TenantStatus.Active,
            tenantType: TenantType.System,
            fiscalCode: fiscalCode,
            phoneNumber: phoneNumber,
            timeZoneOffset: TimeZoneOffset.Default
        );

        var tenantUser = systemTenant.CreateUser(
             name: SystemTenantConstants.Name,
             email: SystemTenantConstants.Email,
             language: Language.Default,
             userState: UserState.Active,
             userStatus: UserStatus.System,
             role: UserRole.Admin,
             phoneNumber: phoneNumber,
             password: password
         );

        systemTenant.SetCreateSession(AnonymousUserConstants.Session_Id);
        tenantUser.SetCreateSession(AnonymousUserConstants.Session_Id);

        tenantUser.SetPropertyValue(x => x.Id, SystemTenantConstants.User_Id);
        systemTenant.SetPropertyValue(x => x.Id, SystemTenantConstants.Tenant_Id);

        _dbContext.Add(systemTenant);
    }
}
