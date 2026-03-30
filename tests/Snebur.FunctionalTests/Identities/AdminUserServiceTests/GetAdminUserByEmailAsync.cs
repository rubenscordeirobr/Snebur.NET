namespace Snebur.FunctionalTests.Identities;

public partial class AdminUserServiceTests
{
    [Fact]
    public async Task GetAdminUserByEmailAsync_ShouldReturnSuccess()
    {
        //Arrange
        var email = DefaultAdminUserConstants.Email;

        //Act
        var result = await _clientService.GetAdminUserByEmailAsync(email, cancellationToken: TestContext.Current.CancellationToken);

        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task GetAdminUserByEmailAsync_ShouldReturnFailureNotFound()
    {
        //Arrange
        var fakeEmail = FakeUtils.GenerateFakeEmail();

        //Act
        var result = await _clientService.GetAdminUserByEmailAsync(fakeEmail, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abc")]
    [InlineData("1")]
    public async Task GetAdminUserByEmailAsync_ShouldReturnFailureNotFoundWhenInvalidOrNull(
        string? email)
    {
        //Act
        var result = await _clientService.GetAdminUserByEmailAsync(email!, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }

    [Fact]
    public async Task GetAdminUserByEmailAsync_ShouldReturnFailureNotFoundWhenInvalidChars( )
    {
        var invalidChars = new char[] { '!', '@', '#', '$', '%', '^', ' ', '&', '*', '(', ')', '-', '+', '=', '{', '}', '[', ']', '|', '\\', ':', ';', '"', '\'', '<', '>', ',', '.', '?', '/', ' ', '\t', '\n', '\r' };
        var invalidCharsString = new string(invalidChars);
        //Act
        var result = await _clientService.GetAdminUserByEmailAsync(invalidCharsString, cancellationToken: TestContext.Current.CancellationToken!);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }

    [Fact]
    public async Task GetAdminUserByEmailAsync_ShouldReturnFailureNotFoundWhenInputHasSpace()
    {
        var inputWithSpace = "test teste2 mail@ .com.br";
        //Act
        var result = await _clientService.GetAdminUserByEmailAsync(inputWithSpace, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldBeFailure<NotFoundError>();
    }
}

