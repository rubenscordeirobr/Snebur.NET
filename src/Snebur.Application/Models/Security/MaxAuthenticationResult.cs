namespace Snebur.Application.Models.Security;
public partial record MaxAuthenticationResult(
    bool IsMaxReached, 
    int CurrentAttempts,
    TimeSpan? ExpirationTime = null);

public partial record MaxAuthenticationResult
{
    public static MaxAuthenticationResult Success
        => new MaxAuthenticationResult(false, 0);
}

