namespace Snebur.Core.Infos;
public partial record ClientRequestHeaderInfo(
    string IpAddress,
    string UserAgent,
    string ApplicationName,
    string? AuthorizationToken);

public partial record ClientRequestHeaderInfo
{
    public static ClientRequestHeaderInfo Unknown
        => new(IpAddress: "Unknown",
               UserAgent: "Unknown",
               ApplicationName: "Unknown",
               null);

    public static ClientRequestHeaderInfo System
        => new(IpAddress: "System",
               UserAgent: "System",
               ApplicationName: "System",
               null);
}
