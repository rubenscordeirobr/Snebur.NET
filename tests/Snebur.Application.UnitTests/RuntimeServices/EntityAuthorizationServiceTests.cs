using Snebur.Persistence.Identity.Extensions;

namespace Snebur.Application.UnitTests.RuntimeServices;

public class EntityAuthorizationServiceTests
{
    private readonly Fixture _figure = new();

    [Fact]
    public void ValidateEntityChange_ShouldNotThrow_WhenAuthorizedAsAnonymous_ForCreateOnAllowedEntity()
    {
        // Arrange
        var service = new EntityAuthorizationService();
        var tenantUser = _figure.Create<TenantUser>();
        var userSession = new FakeUserSession
        {
            Id = Guid.NewGuid(),
            AuthenticationType = AuthenticationType.Anonymous,
            UserRole = UserRole.Anonymous,
            User_Id = Guid.NewGuid()
        };

        // Act
        Action act = () => service.ValidateEntityChange(tenantUser, userSession, EntityChangeState.Created);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateEntityChange_ShouldThrow_WhenAnonymousNotAllowedForNonCreateEntity()
    {
        // Arrange
        var service = new EntityAuthorizationService();
        var fakeEntity = new FakeTenantEntity();
        var userSession = new FakeUserSession
        {
            Id = Guid.NewGuid(),
            AuthenticationType = AuthenticationType.Anonymous,
            UserRole = UserRole.Anonymous,
            User_Id = Guid.NewGuid()
        };

        // Act
        Action act = () => service.ValidateEntityChange(fakeEntity, userSession, EntityChangeState.Updated);

        // Assert
        act.Should().Throw<ForbiddenSecurityException>()
            .WithMessage("*Access Denied*");
    }

    [Fact]
    public void ValidateEntityChange_ShouldThrow_WhenTenantUserIsDifferenceTenant()
    {
        // Arrange
        var service = new EntityAuthorizationService();
        var fakeTenant = new FakeTenant
        {
            Tenant_Id = Guid.NewGuid()
        };
        fakeTenant.SetPropertyValue(x => x.Id, Guid.NewGuid());
        var userSession = new FakeUserSession
        {
            Id = Guid.NewGuid(),
            AuthenticationType = AuthenticationType.Credentials,
            UserRole = UserRole.Viewer,
            Tenant_Id = Guid.NewGuid(),
            User_Id = Guid.NewGuid()
        };

        // Act
        Action act = () => service.ValidateEntityChange(fakeTenant, userSession, EntityChangeState.Updated);

        // Assert
        act.Should().Throw<ForbiddenSecurityException>();

    }

    [Fact]
    public void ValidateEntityChange_ShouldNotThrow_WhenSystemAdmin()
    {
        // Arrange
        var service = new EntityAuthorizationService();
        var fakeEntity = new FakeEntity();
        var userSession = new FakeUserSession
        {
            Id = Guid.NewGuid(),
            AuthenticationType = AuthenticationType.Credentials,
            UserRole = UserRole.SystemAdmin,
            User_Id = Guid.NewGuid(),
            UserType = UserType.SystemUser
        };

        // Act
        Action act = () => service.ValidateEntityChange(fakeEntity, userSession, EntityChangeState.Updated);

        // Assert
        act.Should().NotThrow();
    }

    // Fake types to test anonymous permission
    // These types are allowed for anonymous creation
    public class FakeTenantEntity : EntityBase { }
    public class FakeTenantUser : EntityBase { }
    public class FakeUserSessionEntity : EntityBase { }

    // Fake IUserSession implementation
    public class FakeUserSession : IUserSession
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
        public string ClientSessionToken { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsPersistent { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? TerminatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public Language Language { get; set; }
        public AuthenticationType AuthenticationType { get; set; }
        public SessionTerminationReason? TerminationReason { get; set; }
        public UserRole UserRole { get; set; }
        public UserType UserType { get; set; }
        public GeoLocation? GeoLocation { get; set; }
        public Guid? Tenant_Id { get; set; }
        public Guid User_Id { get; set; }
        public IUser? User { get; set; }

        // Instance methods to simulate extension methods behavior
        public bool IsAnonymous() => AuthenticationType == AuthenticationType.Anonymous;
        public bool IsTenantUser() => Tenant_Id.HasValue;
    }
    public class FakeEntity : EntityBase { }

    public class FakeTenant : EntityBase, ITenantOwned
    {
        public Guid Tenant_Id { get; set; }
    }
}
