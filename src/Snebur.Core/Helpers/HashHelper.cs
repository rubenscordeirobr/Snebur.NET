using System.Security.Cryptography;
using System.Text;

namespace Snebur.Core.Helpers;

public static class HashHelper
{
    public static string CreateSha256Hash(string input)
    {
        return CreateSha256Hash(Encoding.UTF8.GetBytes(input));
    }

    public static string CreateSha256Hash(Guid firstSystemSession_Id)
    {
        return CreateSha256Hash(firstSystemSession_Id.ToByteArray());
    }

    public static string CreateSha256Hash(byte[] bytes)
    {
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hashBytes);
    }

    public static Guid CreateMd5GuidHash(string input)
    {
        return GenerateMd5GuidHash(Encoding.UTF8.GetBytes(input));
    }
     
    public static Guid GenerateMd5GuidHash(byte[] bytes)
    {
        var hashBytes = MD5.HashData(bytes);
        return new Guid(hashBytes);
    }
}
