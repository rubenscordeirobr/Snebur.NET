namespace Snebur.Core.UnitTests.Utils;

public class LocalizedUriUtilsTests
{
    #region BuildLocalizedUri Tests

    [Theory]
    [InlineData("en-US", "products", "/en-us/products")]
    [InlineData("en-US", "/products", "/en-us/products")]
    [InlineData("en-US", "products/", "/en-us/products")]
    [InlineData("en-US", "products//", "/en-us/products")]
    [InlineData("en-US", "/products/", "/en-us/products")]
    [InlineData("en-US", "//products/", "/en-us/products")]
    [InlineData("en-US", "//products//", "/en-us/products")]
    [InlineData("pt-BR", "contato", "/pt-br/contato")]
    [InlineData("pt-BR", "contato/", "/pt-br/contato")]
    [InlineData("pt-BR", "/contato/", "/pt-br/contato")]
    [InlineData("es-ES", "servicios", "/es-es/servicios")]
    [InlineData("fr-FR", "à-propos", "/fr-fr/à-propos")]
    [InlineData("de-DE", "über-uns", "/de-de/über-uns")]
    public void BuildLocalizedUri_WithRelativeUri_ShouldAddCulture(string cultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US", "pt-BR/products", "/en-us/products")]
    [InlineData("en-US", "pt-BR/products/", "/en-us/products")]
    [InlineData("en-US", "/pt-BR/products/", "/en-us/products")]
    [InlineData("en-US", "//pt-BR/products/", "/en-us/products")]
    [InlineData("en-US", "//pt-BR/products//", "/en-us/products")]
    [InlineData("pt-BR", "en-US/contato", "/pt-br/contato")]
    [InlineData("pt-BR", "/en-US/contato", "/pt-br/contato")]
    [InlineData("pt-BR", "en-US/contato/", "/pt-br/contato")]
    [InlineData("pt-BR", "/en-US/contato/", "/pt-br/contato")]
    [InlineData("es-ES", "de-DE/servicios", "/es-es/servicios")]
    public void BuildLocalizedUri_WithRelativeUriAndExistingDifferentCulture_ShouldReplaceCulture(string newCultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, newCultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US", "en-US/products")]
    [InlineData("en-US", "/en-US/products")]
    [InlineData("en-US", "//en-US/products")]
    [InlineData("en-US", "en-US/products/")]
    [InlineData("en-US", "en-US/products//")]
    [InlineData("pt-BR", "pt-BR/contato")]
    [InlineData("es-ES", "es-ES/servicios")]
    public void BuildLocalizedUri_WithRelativeUriAndSameCulture_ShouldReturnOriginalUri
        (string cultureCode, string uri)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode);

        // Assert
        result.Should().Be(uri);
    }

    [Theory]
    [InlineData("en-US", "http://example.com/products", "http://example.com/en-us/products")]
    [InlineData("en-US", "http://example.com/products/", "http://example.com/en-us/products")]
    [InlineData("pt-BR", "https://example.org/contato", "https://example.org/pt-br/contato")]
    [InlineData("es-ES", "https://example.net:8080/servicios", "https://example.net:8080/es-es/servicios")]
    public void BuildLocalizedUri_WithAbsoluteUri_ShouldAddCulture(string cultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US", "http://example.com/pt-BR/products", "http://example.com/en-us/products")]
    [InlineData("en-US", "http://example.com/pt-BR/products/", "http://example.com/en-us/products")]
    [InlineData("en-US", "http://example.com//pt-BR/products/", "http://example.com/en-us/products")]
    [InlineData("pt-BR", "https://example.org/en-US/contato", "https://example.org/pt-br/contato")]
    [InlineData("es-ES", "https://example.net:8080/de-DE/servicios", "https://example.net:8080/es-es/servicios")]
    public void BuildLocalizedUri_WithAbsoluteUriAndExistingCulture_ShouldReplaceCulture(
        string newCultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, newCultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US", "products?key=value", "/en-us/products?key=value")]
    [InlineData("pt-BR", "contato?name=João&email=test@example.com", "/pt-br/contato?name=João&email=test@example.com")]
    [InlineData("es-ES", "servicios?page=1&sort=asc", "/es-es/servicios?page=1&sort=asc")]
    public void BuildLocalizedUri_WithRelativeUriAndQueryString_ShouldPreserveQueryString(string cultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US", "http://example.com/products?key=value", "http://example.com/en-us/products?key=value")]
    [InlineData("en-US", "http://example.com//products?key=value", "http://example.com/en-us/products?key=value")]
    [InlineData("pt-BR", "https://example.org/contato?name=João&email=test@example.com", "https://example.org/pt-br/contato?name=João&email=test@example.com")]
    [InlineData("es-ES", "https://example.net:8080/servicios?page=1&sort=asc", "https://example.net:8080/es-es/servicios?page=1&sort=asc")]
    public void BuildLocalizedUri_WithAbsoluteUriAndQueryString_ShouldPreserveUriAndQueryString(string cultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US", "pt-BR/products?key=value", "/en-us/products?key=value")]
    [InlineData("pt-BR", "en-US/contato?name=João&email=test@example.com", "/pt-br/contato?name=João&email=test@example.com")]
    public void BuildLocalizedUri_WithRelativeUriContainingCultureAndQueryString_ShouldReplaceCultureAndPreserveQueryString(string newCultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, newCultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US", "http://example.com/pt-BR/products?key=value", "http://example.com/en-us/products?key=value")]
    [InlineData("pt-BR", "https://example.org/en-US/contato?name=João&email=test@example.com", "https://example.org/pt-br/contato?name=João&email=test@example.com")]
    public void BuildLocalizedUri_WithAbsoluteUriContainingCultureAndQueryString_ShouldReplaceCultureAndPreserveQueryString(string newCultureCode, string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, newCultureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, "en-US")]
    [InlineData("", "en-US")]
    [InlineData("   ", "en-US")]
    public void BuildLocalizedUri_WithNullOrEmptyUri_ShouldReturnOnlyCulturePath(
        string? uri, string cultureCode)
    {
        // Act
        var result = LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode);

        // Assert
        result.Should().Be($"/{cultureCode.ToLowerInvariant()}");
    }

    [Theory]
    [InlineData("products", "invalid-culture")]
    [InlineData("contato", "xx-YY")]
    [InlineData("servicios", "")]
    public void BuildLocalizedUri_WithUnsupportedCulture_ShouldThrowInvalidOperationException(string uri, string cultureCode)
    {
        // Act
        Action act = () => LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("products", null)]
    public void BuildLocalizedUri_WithNullCultureCode_ShouldThrowArgumentNullException(string? uri, string? cultureCode)
    {
        // Act
        Action act = () => LocalizedUriUtils.BuildLocalizedUri(uri, cultureCode!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region BuildDefaultCultureUri Tests

    [Theory]
    [InlineData("products", "/en-us/products")]
    [InlineData("pt-BR/contato", "/en-us/contato")]
    [InlineData("http://example.com/products", "http://example.com/en-us/products")]
    [InlineData("http://example.com/pt-BR/products", "http://example.com/en-us/products")]
    public void BuildDefaultCultureUri_ShouldUseEnUsAsDefaultCulture(string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildDefaultCultureUri(uri);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("products", "key=value", "/en-us/products?key=value")]
    [InlineData("contato", "name=João&email=test@example.com", "/en-us/contato?name=João&email=test@example.com")]
    [InlineData("http://example.com/products", "key=value", "http://example.com/en-us/products?key=value")]
    [InlineData("http://example.com/pt-BR/products", "key=value", "http://example.com/en-us/products?key=value")]
    public void BuildDefaultCultureUri_WithQueryString_ShouldAppendQueryString(string uri, string queryString, string expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildDefaultCultureUri(uri, queryString);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("products", null, "/en-us/products")]
    [InlineData("/products", null, "/en-us/products")]
    [InlineData("/products//", null, "/en-us/products")]
    [InlineData("products", "", "/en-us/products")]
    [InlineData("products", "   ", "/en-us/products")]
    [InlineData("//products//", "   ", "/en-us/products")]
    public void BuildDefaultCultureUri_WithNullOrEmptyQueryString_ShouldNotAppendQueryString(
        string? uri, string? queryString, string? expected)
    {
        // Act
        var result = LocalizedUriUtils.BuildDefaultCultureUri(uri, queryString);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BuildDefaultCultureUri_WithNullOrEmptyUri_ShouldReturnOnlyCulturePath(string? uri)
    {
        // Act
        var result = LocalizedUriUtils.BuildDefaultCultureUri(uri!);

        // Assert
        result.Should().Be($"/en-us");
    }

    #endregion

    #region ExtractCultureCodeFromUri Tests

    [Theory]
    [InlineData("en-US/products", "en-us")]
    [InlineData("pt-BR/contato", "pt-br")]
    [InlineData("es-ES/servicios", "es-es")]
    [InlineData("fr-FR/à-propos", "fr-fr")]
    [InlineData("de-DE/über-uns", "de-de")]
    [InlineData("/en-US/products", "en-us")]
    [InlineData("/pt-BR/contato/", "pt-br")]
    [InlineData("/es-ES/servicios/details", "es-es")]
    public void ExtractCultureCodeFromUri_WithRelativeUriAndSupportedCulture_ShouldReturnCultureCode(string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("http://example.com/en-US/products", "en-us")]
    [InlineData("https://example.org/pt-BR/contato", "pt-br")]
    [InlineData("https://example.net:8080/es-ES/servicios", "es-es")]
    [InlineData("http://example.com/fr-FR/à-propos", "fr-fr")]
    [InlineData("https://example.org/de-DE/über-uns", "de-de")]
    public void ExtractCultureCodeFromUri_WithAbsoluteUriAndSupportedCulture_ShouldReturnCultureCode(string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("products")]
    [InlineData("contato/details")]
    [InlineData("/products")]
    [InlineData("/contato/details")]
    [InlineData("/sobre-nós")]
    public void ExtractCultureCodeFromUri_WithRelativeUriWithoutCulture_ShouldReturnNull(string uri)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("http://example.com/products")]
    [InlineData("https://example.org/contato/details")]
    [InlineData("https://example.net:8080/sobre-nós")]
    public void ExtractCultureCodeFromUri_WithAbsoluteUriWithoutCulture_ShouldReturnNull(string uri)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("invalid-culture/products")]
    [InlineData("xx-YY/contato")]
    [InlineData("/invalid/details")]
    [InlineData("/not-a-culture-code/page")]
    public void ExtractCultureCodeFromUri_WithRelativeUriAndUnsupportedCulture_ShouldReturnNull(string uri)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("http://example.com/invalid-culture/products")]
    [InlineData("https://example.org/xx-YY/contato")]
    [InlineData("https://example.net:8080/not-a-culture-code/page")]
    public void ExtractCultureCodeFromUri_WithAbsoluteUriAndUnsupportedCulture_ShouldReturnNull(string uri)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("//en-US")]
    [InlineData("/pt-BR")]
    [InlineData("/es-ES/")]
    [InlineData("/es-ES//")]
    public void ExtractCultureCodeFromUri_WithOnlyCultureCode_ShouldReturnCultureCode(string uri)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(uri.Trim('/').ToLowerInvariant());
    }

    [Theory]
    [InlineData("http://example.com/en-US?param=value", "en-us")]
    [InlineData("https://example.org/pt-BR/contato?name=João&email=test@example.com", "pt-br")]
    [InlineData("https://example.net:8080/es-ES/servicios?page=1&sort=asc", "es-es")]
    public void ExtractCultureCodeFromUri_WithAbsoluteUriAndQueryString_ShouldReturnCultureCode(string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("en-US/products?param=value", "en-us")]
    [InlineData("/pt-BR/contato?name=João&email=test@example.com", "pt-br")]
    [InlineData("/es-ES/servicios?page=1&sort=asc", "es-es")]
    public void ExtractCultureCodeFromUri_WithRelativeUriAndQueryString_ShouldReturnCultureCode(string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ExtractCultureCodeFromUri_WithNullUri_ShouldReturnNull()
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(null);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ExtractCultureCodeFromUri_WithEmptyUri_ShouldReturnNull(string uri)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("EN-US/products", "en-us")]
    [InlineData("PT-br/contato", "pt-br")]
    [InlineData("/Es-ES/servicios", "es-es")]
    public void ExtractCultureCodeFromUri_WithMixedCaseUri_ShouldPreserveCaseInResult(string uri, string expected)
    {
        // Act
        var result = LocalizedUriUtils.ExtractCultureCodeFromUri(uri);

        // Assert
        result.Should().Be(expected);
    }

    #endregion
}
