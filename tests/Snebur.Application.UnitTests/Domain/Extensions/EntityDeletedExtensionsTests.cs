using Snebur.SharedKernel.Abstractions;
using Moq;

namespace Snebur.Application.UnitTests.Domain.Extensions;

public class EntityDeletedExtensionsTests
{
    private Fixture _figure = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "ValidSession")]
    [InlineData("ValidEntity", null)]
    public void MarkAsDeleted_Should_ThrowArgumentNullException_When_Inputs_Are_Null(
        string? entityName,
        string? sessionName)
    {
        // Arrange
        ISoftDeletableEntity? entity = entityName == null
            ? null
            : new Mock<ISoftDeletableEntity>().Object;

        IUserSession? userSession = sessionName == null
            ? null
            : new Mock<IUserSession>().Object;

        // Act
        Action act = () => entity!.MarkAsDeleted(userSession!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MarkAsDeleted_Should_ThrowInvalidOperationException_When_Session_IsAnonymous()
    {
        // Arrange
        var entityMock = new Mock<ISoftDeletableEntity>();
        var userSessionMock = new Mock<IUserSession>();

        userSessionMock.Setup(s => s.User_Id).Returns(AnonymousUserConstants.User_Id);
        var userSession = userSessionMock.Object;

        // Act
        Action act = () => entityMock.Object.MarkAsDeleted(userSession);

        // Assert
        act.Should().Throw<ForbiddenSecurityException>();
    }

    [Fact]
    public void MarkAsDeleted_Should_NotThrow_When_Session_IsNotAnonymous()
    {
        // Arrange
        var entityTest = _figure.Create<MockSoftDeletableEntity>();
        
        var userSessionMock = new Mock<IUserSession>();
        userSessionMock.Setup(s => s.User_Id).Returns(Guid.NewGuid());
        userSessionMock.Setup(s => s.UserType).Returns(UserType.AdminUser);
        var userSession = userSessionMock.Object;

        // Act
        Action act = () => entityTest.MarkAsDeleted(userSession);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void MarkAsDeleted_Should_ThrowInvalidOperationException_When_EntityTenantDiffersFromUserSession()
    {
        // Arrange
        var entityMock = new Mock<ISoftDeletableEntity>();

        // Mock IEntityTenant behavior
        var tenantId = Guid.NewGuid();
        var tenantEntity = entityMock.As<ITenantOwned>();
        tenantEntity.Setup(e => e.Tenant_Id).Returns(tenantId);

        // Mock IUserSession with a different Tenant_Id
        var userSessionMock = new Mock<IUserSession>();
        userSessionMock.Setup(s => s.Tenant_Id).Returns(Guid.NewGuid());
        userSessionMock.Setup(s => s.UserType).Returns(UserType.TenantUser);

        // Act
        Action act = () => entityMock.Object.MarkAsDeleted(userSessionMock.Object);

        // Assert
        act.Should().Throw<ForbiddenSecurityException>();
    }
     
    [Fact]
    public void MarkAsDeleted_Should_MarkEntityAsDeleted_When_Conditions_AreMet()
    {
        // Arrange
        var entity = _figure.Create<Tenant>();

        var userSessionId = Guid.NewGuid();

        var userSessionMock = new Mock<IUserSession>();
        userSessionMock.Setup(s => s.User_Id).Returns(Guid.NewGuid());
        userSessionMock.Setup(s => s.Id).Returns(userSessionId);
        userSessionMock.Setup(s => s.UserType).Returns(UserType.AdminUser);

        var userSession = userSessionMock.Object;

        // Act
        entity.MarkAsDeleted(userSession);

        // Assert
        entity.DeletedAt.Should()
            .BeAfter(DateTime.UtcNow.AddSeconds(-1))
            .And
            .BeBefore(DateTime.UtcNow.AddSeconds(1));

        entity.DeletedSession_Id
            .Should()
            .Be(userSessionId);

        entity.IsDeleted
            .Should()
            .BeTrue();
    }

    private class MockSoftDeletableEntity : ISoftDeletableEntity
    {
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Guid? DeletedSession_Id { get; private set; }
    }
}
