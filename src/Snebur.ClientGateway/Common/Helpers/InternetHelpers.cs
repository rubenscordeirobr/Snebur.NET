
using System.Net.NetworkInformation;

namespace Snebur.ClientGateway.Common.Helpers;

public static class InternetHelpers
{
    internal static bool HasInternetConnection()
    {
        try
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
        catch
        {
            return false;
        }
    }

    internal static Task WaitForInternetConnectionAsync()
    {
        throw new NotImplementedException();
    }
}

