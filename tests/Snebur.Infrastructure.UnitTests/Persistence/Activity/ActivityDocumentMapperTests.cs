using Snebur.Domain.Entities.Activities;
using Snebur.Persistence.Activity.Documents;

namespace Snebur.Infrastructure.UnitTests.Persistence.Activity;

public class ActivityDocumentMapperTests
{
    [Fact]
    public void MapToDocument_ShouldMapCreatedActivityCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var createdActivity = new CreatedActivity
        {
            Id = "activity1",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityDate = activityDate,
            Description = "Created activity description",
            CreatedData = "Data Created",
            QualifiedTypeName = "TestEntity",
            Entity_Id = Guid.NewGuid()
        };

        // Act
        var document = ActivityDocumentMapper.MapToDocument(createdActivity);

        // Assert
        document.Tenant_Id.Should().Be(tenantId);
        document.UserSession_Id.Should().Be(userSessionId);
        document.Description.Should().Be(createdActivity.Description);
        document.ActivityType.Should().Be(ActivityType.Created);
        document.CreatedData.Should().Be("Data Created");
        document.QualifiedTypeName.Should().Be("TestEntity");
        document.Entity_Id.Should().Be(createdActivity.Entity_Id);
    }

    [Fact]
    public void MapToDocument_ShouldMapUpdatedActivityCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var updatedActivity = new UpdatedActivity
        {
            Id = "activity2",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityDate = activityDate,
            Description = "Updated activity description",
            OldData = "Old",
            NewData = "New",
            QualifiedTypeName = "TestEntity",
            Entity_Id = Guid.NewGuid()
        };

        // Act
        var document = ActivityDocumentMapper.MapToDocument(updatedActivity);

        // Assert
        document.Tenant_Id.Should().Be(tenantId);
        document.UserSession_Id.Should().Be(userSessionId);
        document.Description.Should().Be(updatedActivity.Description);
        document.ActivityType.Should().Be(ActivityType.Updated);
        document.OldData.Should().Be("Old");
        document.NewData.Should().Be("New");
        document.QualifiedTypeName.Should().Be("TestEntity");
        document.Entity_Id.Should().Be(updatedActivity.Entity_Id);
    }

    [Fact]
    public void MapToDocument_ShouldMapDeletedActivityCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var deletedActivity = new DeletedActivity
        {
            Id = "activity3",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityDate = activityDate,
            Description = "Deleted activity description",
            DeletedData = "Data Deleted",
            QualifiedTypeName = "TestEntity",
            Entity_Id = Guid.NewGuid()
        };

        // Act
        var document = ActivityDocumentMapper.MapToDocument(deletedActivity);

        // Assert
        document.Tenant_Id.Should().Be(tenantId);
        document.UserSession_Id.Should().Be(userSessionId);
        document.Description.Should().Be(deletedActivity.Description);
        document.ActivityType.Should().Be(ActivityType.Deleted);
        document.DeletedData.Should().Be("Data Deleted");
        document.QualifiedTypeName.Should().Be("TestEntity");
        document.Entity_Id.Should().Be(deletedActivity.Entity_Id);
    }

    [Fact]
    public void MapToDocument_ShouldMapLoginSuccessfulActivityCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var loginSuccessfulActivity = new UserLoginSuccessActivity
        {
            Id = "activity4",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityDate = activityDate,
            Description = "Login success",
            IpAddress = "127.0.0.1",
            AuthenticationType = AuthenticationType.Credentials,
            UserIdentifier = "email@email.com",
            User_Id = Guid.NewGuid(),
        };

        // Act
        var document = ActivityDocumentMapper.MapToDocument(loginSuccessfulActivity);

        // Assert
        document.Tenant_Id.Should().Be(tenantId);
        document.UserSession_Id.Should().Be(userSessionId);
        document.Description.Should().Be(loginSuccessfulActivity.Description);
        document.ActivityType.Should().Be(ActivityType.UserLoginSuccess);
        document.IpAddress.Should().Be("127.0.0.1");
        document.AuthenticationType.Should().Be(AuthenticationType.Credentials);
    }

    [Fact]
    public void MapToDocument_ShouldMapLoginFailedActivityCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var loginFailedActivity = new UserLoginFailureActivity
        {
            Id = "activity5",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityDate = activityDate,
            Description = "Login failed",
            IpAddress = "192.168.1.1",
            PasswordFailed = "WrongPassword",
            UserIdentifier = "email@email.com",
            User_Id = Guid.NewGuid(),
            AuthenticationType = AuthenticationType.Credentials
        };

        // Act
        var document = ActivityDocumentMapper.MapToDocument(loginFailedActivity);

        // Assert
        document.Tenant_Id.Should().Be(tenantId);
        document.UserSession_Id.Should().Be(userSessionId);
        document.Description.Should().Be(loginFailedActivity.Description);
        document.ActivityType.Should().Be(ActivityType.UserLoginFailed);
        document.IpAddress.Should().Be("192.168.1.1");
        document.PasswordFailed.Should().Be("WrongPassword");
    }

    [Fact]
    public void MapToDomain_ShouldMapCreatedDocumentToCreatedActivity()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var document = new ActivityDocument
        {
            Id = "doc1",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityAt = activityDate,
            Description = "Created doc",
            ActivityType = ActivityType.Created,
            CreatedData = "Data Created",
            QualifiedTypeName = "TestEntity",
            Entity_Id = Guid.NewGuid()
        };

        // Act
        var activity = ActivityDocumentMapper.MapToDomain(document);

        // Assert
        activity.Should().BeOfType<CreatedActivity>();

        var createdActivity = activity as CreatedActivity;
        createdActivity.Should()
            .NotBeNull();

        Guard.NotNull(createdActivity);

        createdActivity.Id.Should().Be("doc1");
        createdActivity.Tenant_Id.Should().Be(tenantId);
        createdActivity.UserSession_Id.Should().Be(userSessionId);
        createdActivity.ActivityDate.Should().Be(activityDate);
        createdActivity.Description.Should().Be("Created doc");
        createdActivity.CreatedData.Should().Be("Data Created");
        createdActivity.QualifiedTypeName.Should().Be("TestEntity");
        createdActivity.Entity_Id.Should().Be(document.Entity_Id.GetValueOrDefault());
    }

    [Fact]
    public void MapToDomain_ShouldMapUpdatedDocumentToUpdatedActivity()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var document = new ActivityDocument
        {
            Id = "doc2",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityAt = activityDate,
            Description = "Updated doc",
            ActivityType = ActivityType.Updated,
            OldData = "Old",
            NewData = "New",
            QualifiedTypeName = "TestEntity",
            Entity_Id = Guid.NewGuid()
        };

        // Act
        var activity = ActivityDocumentMapper.MapToDomain(document);

        // Assert
        activity.Should().BeOfType<UpdatedActivity>();
        var updatedActivity = activity as UpdatedActivity;

        updatedActivity.Should().NotBeNull();

        Guard.NotNull(updatedActivity);

        updatedActivity.Id.Should().Be("doc2");
        updatedActivity.Tenant_Id.Should().Be(tenantId);
        updatedActivity.UserSession_Id.Should().Be(userSessionId);
        updatedActivity.ActivityDate.Should().Be(activityDate);
        updatedActivity.Description.Should().Be("Updated doc");
        updatedActivity.OldData.Should().Be("Old");
        updatedActivity.NewData.Should().Be("New");
        updatedActivity.QualifiedTypeName.Should().Be("TestEntity");
        updatedActivity.Entity_Id.Should().Be(document.Entity_Id.GetValueOrDefault());
    }

    [Fact]
    public void MapToDomain_ShouldMapDeletedDocumentToDeletedActivity()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var document = new ActivityDocument
        {
            Id = "doc3",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityAt = activityDate,
            Description = "Deleted doc",
            ActivityType = ActivityType.Deleted,
            DeletedData = "Data Deleted",
            QualifiedTypeName = "TestEntity",
            Entity_Id = Guid.NewGuid()
        };

        // Act
        var activity = ActivityDocumentMapper.MapToDomain(document);

        // Assert
        activity.Should().BeOfType<DeletedActivity>();

        var deletedActivity = activity as DeletedActivity;
        deletedActivity.Should().NotBeNull();

        Guard.NotNull(deletedActivity);

        deletedActivity.Id.Should().Be("doc3");
        deletedActivity.Tenant_Id.Should().Be(tenantId);
        deletedActivity.UserSession_Id.Should().Be(userSessionId);
        deletedActivity.ActivityDate.Should().Be(activityDate);
        deletedActivity.Description.Should().Be("Deleted doc");
        deletedActivity.DeletedData.Should().Be("Data Deleted");
        deletedActivity.QualifiedTypeName.Should().Be("TestEntity");
        deletedActivity.Entity_Id.Should().Be(document.Entity_Id.GetValueOrDefault());
    }

    [Fact]
    public void MapToDomain_ShouldMapLoginSuccessfulDocumentToLoginSuccessfulActivity()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var document = new ActivityDocument
        {
            Id = "doc4",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityAt = activityDate,
            Description = "Login success doc",
            ActivityType = ActivityType.UserLoginSuccess,
            IpAddress = "127.0.0.1",
            AuthenticationType = AuthenticationType.Google
        };

        // Act
        var activity = ActivityDocumentMapper.MapToDomain(document);

        // Assert
        activity.Should().BeOfType<UserLoginSuccessActivity>();
        var loginActivity = activity as UserLoginSuccessActivity;
        loginActivity.Should().NotBeNull();

        Guard.NotNull(loginActivity);

        loginActivity.Id.Should().Be("doc4");
        loginActivity.Tenant_Id.Should().Be(tenantId);
        loginActivity.UserSession_Id.Should().Be(userSessionId);
        loginActivity.ActivityDate.Should().Be(activityDate);
        loginActivity.Description.Should().Be("Login success doc");
        loginActivity.IpAddress.Should().Be("127.0.0.1");
        loginActivity.AuthenticationType.Should().Be(AuthenticationType.Google);
    }

    [Fact]
    public void MapToDomain_ShouldMapLoginFailedDocumentToLoginFailedActivity()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userSessionId = Guid.NewGuid();
        var activityDate = DateTime.UtcNow;
        var document = new ActivityDocument
        {
            Id = "doc5",
            Tenant_Id = tenantId,
            UserSession_Id = userSessionId,
            ActivityAt = activityDate,
            Description = "Login failed doc",
            ActivityType = ActivityType.UserLoginFailed,
            IpAddress = "192.168.1.1",
            PasswordFailed = "WrongPassword"
        };

        // Act
        var activity = ActivityDocumentMapper.MapToDomain(document);

        // Assert
        activity.Should().BeOfType<UserLoginFailureActivity>();
        var failedActivity = activity as UserLoginFailureActivity;
        failedActivity.Should().NotBeNull();

        Guard.NotNull(failedActivity);

        failedActivity.Id.Should().Be("doc5");
        failedActivity.Tenant_Id.Should().Be(tenantId);
        failedActivity.UserSession_Id.Should().Be(userSessionId);
        failedActivity.ActivityDate.Should().Be(activityDate);
        failedActivity.Description.Should().Be("Login failed doc");
        failedActivity.IpAddress.Should().Be("192.168.1.1");
        failedActivity.PasswordFailed.Should().Be("WrongPassword");
    }
}
