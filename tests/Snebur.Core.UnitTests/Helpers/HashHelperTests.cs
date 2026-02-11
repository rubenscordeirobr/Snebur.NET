using System.Security.Cryptography;
using System.Text;
using Snebur.Core.Helpers;

namespace Snebur.Core.UnitTests.Helpers;
public class HashHelperTests
{
    [Theory]
    [InlineData("test", "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08")]
    [InlineData("hello", "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824")]
    public void GenerateSha256HashFromString_ShouldReturnExpectedHash(string input, string expectedHash)
    {
        var result = HashHelper.CreateSha256Hash(input);
        result.Should().Be(expectedHash);
    }

    [Fact]
    public void CreateSha256Hash_ShouldReturnExpectedHash()
    {
        var guid = Guid.NewGuid();
        var expectedHash = HashHelper.CreateSha256Hash(guid.ToByteArray());
        var result = HashHelper.CreateSha256Hash(guid);
        result.Should().Be(expectedHash);
    }

    [Theory]
    [InlineData("test", "cd6b8f09214673d3cade4e832627b4f6")]
    [InlineData("hello", "2a40415d4bbc762ab9719d911017c592")]
    public void CreateMd5GuidHash_ShouldReturnExpectedGuid(string input, string expectedHash)
    {
        var expectedGuid = new Guid(expectedHash);
        var result = HashHelper.CreateMd5GuidHash(input);
        result.Should().Be(expectedGuid);
    }

    [Fact]
    public void GenerateMd5GuidHash_ShouldReturnExpectedGuid()
    {
        var bytes = Encoding.UTF8.GetBytes("test");
        var expectedGuid = new Guid(MD5.Create().ComputeHash(bytes));
        var result = HashHelper.GenerateMd5GuidHash(bytes);
        result.Should().Be(expectedGuid);
    }
}
