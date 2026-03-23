namespace Snebur.Testing.Core.Mocks;

public interface IRoleProvider
{
    UserRole UserRole { get; }
}
public class AnonymousRole : IRoleProvider
{
    public UserRole UserRole => UserRole.Anonymous;
}

public class AdminUserRole : IRoleProvider
{
    public UserRole UserRole => UserRole.Admin;
}

public class TenantOwnerRole : IRoleProvider
{
    public UserRole UserRole => UserRole.Owner;
}
