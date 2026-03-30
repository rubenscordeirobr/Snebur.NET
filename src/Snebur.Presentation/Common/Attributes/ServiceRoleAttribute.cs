namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ServiceRoleAttribute : Attribute
{
    public ServiceRole ServiceRole { get; }

    public ServiceRoleAttribute(ServiceRole serviceRole)
    {
        ServiceRole = serviceRole;
    }
}

