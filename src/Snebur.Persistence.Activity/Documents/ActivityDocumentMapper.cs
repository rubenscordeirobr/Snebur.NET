using Snebur.Core;
using Snebur.Domain.Entities.Activities;
using Snebur.SharedKernel.Enums;

namespace Snebur.Persistence.Activity.Documents;

public static class ActivityDocumentMapper
{
    public static ActivityDocument MapToDocument(ActivityBase activity)
    {
        Guard.NotNull(activity);

        var document = new ActivityDocument
        {
            Tenant_Id = activity.Tenant_Id,
            UserSession_Id = activity.UserSession_Id,
            ActivityType = activity.ActivityType,
            Description = activity.Description,
        };

        switch (activity)
        {
            case CreatedActivity createActivity:
                document.CreatedData = createActivity.CreatedData;
                break;
            case UpdatedActivity updateActivity:
                document.OldData = updateActivity.OldData;
                document.NewData = updateActivity.NewData;
                break;
            case DeletedActivity deleteActivity:
                document.DeletedData = deleteActivity.DeletedData;
                break;
            case UserLoginSuccessActivity authActivity:
                document.UserIdentifier = authActivity.UserIdentifier;
                document.IpAddress = authActivity.IpAddress;
                document.AuthenticationType = authActivity.AuthenticationType;
                break;
            case UserLoginFailureActivity failedAuthActivity:

                document.UserIdentifier = failedAuthActivity.UserIdentifier;
                document.IpAddress = failedAuthActivity.IpAddress;
                document.PasswordFailed = failedAuthActivity.PasswordFailed;
                document.AuthenticationType = failedAuthActivity.AuthenticationType;
                break;
        }

        if (activity is EntityActivity entityActivity)
        {
            document.QualifiedTypeName = entityActivity.QualifiedTypeName;
            document.Entity_Id = entityActivity.Entity_Id;
        }

        return document;
    }

    public static ActivityBase MapToDomain(ActivityDocument document)
    {
        Guard.NotNull(document);

        switch (document.ActivityType)
        {
            case ActivityType.Created:

                return new CreatedActivity
                {
                    Id = document.Id,
                    Tenant_Id = document.Tenant_Id,
                    UserSession_Id = document.UserSession_Id,
                    ActivityDate = document.ActivityAt,
                    Description = document.Description,
                    CreatedData = document.CreatedData ?? string.Empty,
                    QualifiedTypeName = document.QualifiedTypeName ?? string.Empty,
                    Entity_Id = document.Entity_Id.GetValueOrDefault()
                };

            case ActivityType.Updated:

                return new UpdatedActivity
                {
                    Id = document.Id,
                    Tenant_Id = document.Tenant_Id,
                    UserSession_Id = document.UserSession_Id,
                    Description = document.Description,
                    ActivityDate = document.ActivityAt,
                    OldData = document.OldData ?? string.Empty,
                    NewData = document.NewData ?? string.Empty,
                    QualifiedTypeName = document.QualifiedTypeName ?? string.Empty,
                    Entity_Id = document.Entity_Id.GetValueOrDefault()
                };

            case ActivityType.Deleted:

                return new DeletedActivity
                {
                    Id = document.Id,
                    Tenant_Id = document.Tenant_Id,
                    UserSession_Id = document.UserSession_Id,
                    Description = document.Description,
                    ActivityDate = document.ActivityAt,
                    DeletedData = document.DeletedData ?? string.Empty,
                    QualifiedTypeName = document.QualifiedTypeName ?? string.Empty,
                    Entity_Id = document.Entity_Id.GetValueOrDefault()
                };

            case ActivityType.UserLoginSuccess:

                return new UserLoginSuccessActivity
                {
                    Id = document.Id,
                    Tenant_Id = document.Tenant_Id,
                    UserSession_Id = document.UserSession_Id,
                    Description = document.Description,
                    ActivityDate = document.ActivityAt,
                    AuthenticationType = document.AuthenticationType ?? AuthenticationType.Unknown,
                    IpAddress = document.IpAddress ?? "Unknown",
                    UserIdentifier = document.UserIdentifier!,
                    User_Id = document.Entity_Id.GetValueOrDefault()
                };

            case ActivityType.UserLoginFailed:

                return new UserLoginFailureActivity
                {
                    Id = document.Id,
                    Tenant_Id = document.Tenant_Id,
                    UserSession_Id = document.UserSession_Id,
                    Description = document.Description,
                    ActivityDate = document.ActivityAt,
                    IpAddress = document.IpAddress ?? "Unknown",
                    PasswordFailed = document.PasswordFailed ?? "Unknown",
                    UserIdentifier = document.UserIdentifier!,
                    User_Id = document.Entity_Id.GetValueOrDefault(),
                    AuthenticationType = document.AuthenticationType ?? AuthenticationType.Unknown
                };
            case ActivityType.UserLogout:

                return new UserLogoutActivity
                {
                    Id = document.Id,
                    Tenant_Id = document.Tenant_Id,
                    UserSession_Id = document.UserSession_Id,
                    Description = document.Description,
                    ActivityDate = document.ActivityAt,
                    IpAddress = document.IpAddress ?? "Unknown",
                    User_Id = document.Entity_Id.GetValueOrDefault()
                };
            default:

                throw new ArgumentException($"Unsupported ActivityType: {document.ActivityType}");
        }
    }
}
