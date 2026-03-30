namespace Snebur.FunctionalTests.Identities;

public partial class TenantUserServiceTests
{
    [Fact]
    public async Task GetTenantUserByEmailAsync_ShouldReturnSuccess()
    {
        //Arrange
        var email = SystemTenantConstants.Email;

        //Act
        var result = await _clientService.GetTenantUserByEmailAsync(email, cancellationToken: TestContext.Current.CancellationToken);

        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task GetTenantUserByEmailAsync_ShouldReturnFailureNotFound()
    {
        //Arrange
        var fakeEmail = FakeUtils.GenerateFakeEmail();

        //Act
        var result = await _clientService.GetTenantUserByEmailAsync(fakeEmail, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abc")]
    [InlineData("1")]
    public async Task GetTenantUserByEmailAsync_ShouldReturnFailureNotFoundWhenInvalidOrNull(
        string? email)
    {
        //Act
        var result = await _clientService.GetTenantUserByEmailAsync(email!, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }

    [Fact]
    public async Task GetTenantUserByEmailAsync_ShouldReturnFailureNotFoundWhenInvalidChars( )
    {
        var invalidChars = new char[] { '!', '@', '#', '$', '%', '^', ' ', '&', '*', '(', ')', '-', '+', '=', '{', '}', '[', ']', '|', '\\', ':', ';', '"', '\'', '<', '>', ',', '.', '?', '/', ' ', '\t', '\n', '\r' };
        var invalidCharsString = new string(invalidChars);
        //Act
        var result = await _clientService.GetTenantUserByEmailAsync(invalidCharsString!, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }

    [Fact]
    public async Task GetTenantUserByEmailAsync_ShouldReturnFailureNotFoundWhenInputHasSpace()
    {
        var inputWithSpace = "test teste2 mail@ .com.br";
        //Act
        var result = await _clientService.GetTenantUserByEmailAsync(inputWithSpace, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }
}
