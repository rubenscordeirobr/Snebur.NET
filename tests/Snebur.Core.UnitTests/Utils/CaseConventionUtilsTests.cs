namespace Snebur.Core.UnitTests.Utils;

public class CaseConventionUtilsTests
{
    [Theory]

    [InlineData("lowercase", CaseType.LowerCase)]
    [InlineData("UPPERCASE", CaseType.UpperCase)]
    [InlineData("PascalCaseExample", CaseType.PascalCase)]
    [InlineData("UUPascalCaseExample", CaseType.PascalCase)]
    [InlineData("camelCaseExample", CaseType.CamelCase)]
    [InlineData("snake_case_example", CaseType.SnakeCase)]
    [InlineData("kebab-case-example", CaseType.KebabCase)]
    [InlineData("UPPER_CASE_EXAMPLE", CaseType.ScreamingSnakeCase)]
    [InlineData("UPPER-KEBEB", CaseType.ScreamingKebabCase)]
    [InlineData("Pascal_Snake_Case", CaseType.TitleSnakeCase)]
    [InlineData("Pascal-Kebab-Case", CaseType.TitleKebabCase)]
    [InlineData("camel_Snake_Case", CaseType.CamelSnakeCase)]
    [InlineData("camel-Kebab-Case", CaseType.CamelKebabCase)]
    [InlineData("unknown%caseexample", CaseType.Unknown)]
    [InlineData("lower%lower", CaseType.Unknown)]
    [InlineData("UPPPR%lower", CaseType.Unknown)]
    public void GetCaseType_ShouldReturnCorrectCaseType(string input, CaseType expected)
    {
        var result = CaseConventionUtils.GetCaseType(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("PascalCaseExample", "pascal_case_example")]
    [InlineData("UUPascalCaseExample", "uu_pascal_case_example")]
    [InlineData("camelCaseExample", "camel_case_example")]
    [InlineData("snake_case_example", "snake_case_example")]
    [InlineData("kebab-case-example", "kebab_case_example")]
    [InlineData("UPPER_CASE_EXAMPLE", "upper_case_example")]
    public void ToSnakeCase_ShouldConvertToSnakeCase(string input, string expected)
    {
        var result = CaseConventionUtils.ToSnakeCase(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("PascalCaseExample", "pascal-case-example")]
    [InlineData("UUPascalCaseExample", "uu-pascal-case-example")]
    [InlineData("camelCaseExample", "camel-case-example")]
    [InlineData("snake_case_example", "snake-case-example")]
    [InlineData("kebab-case-example", "kebab-case-example")]
    [InlineData("UPPER_CASE_EXAMPLE", "upper-case-example")]
    public void ToKebabCase_ShouldConvertToKebabCase(string input, string expected)
    {
        var result = CaseConventionUtils.ToKebabCase(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("pascal_case_example", "PascalCaseExample")]
    [InlineData("camel_case_example", "CamelCaseExample")]
    [InlineData("snake_case_example", "SnakeCaseExample")]
    [InlineData("kebab-case-example", "KebabCaseExample")]
    [InlineData("upper_case_example", "UpperCaseExample")]
    public void ToPascalCase_ShouldConvertToPascalCase(string input, string expected)
    {
        var result = CaseConventionUtils.ToPascalCase(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("PascalCaseExample", "pascalCaseExample")]
    [InlineData("camel_case_example", "camelCaseExample")]
    [InlineData("snake_case_example", "snakeCaseExample")]
    [InlineData("kebab-case-example", "kebabCaseExample")]
    [InlineData("upper_case_example", "upperCaseExample")]
    public void ToCamelCase_ShouldConvertToCamelCase(string input, string expected)
    {
        var result = CaseConventionUtils.ToCamelCase(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("PascalCaseExample", "PASCALCASEEXAMPLE")]
    [InlineData("camelCaseExample", "CAMELCASEEXAMPLE")]
    [InlineData("snake_case_example", "SNAKECASEEXAMPLE")]
    [InlineData("kebab-case-example", "KEBABCASEEXAMPLE")]
    [InlineData("upper_case_example", "UPPERCASEEXAMPLE")]
    public void ToUpperCase_ShouldConvertToUpperCase(string input, string expected)
    {
        var result = CaseConventionUtils.ToUpperCase(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetCaseType_ShouldThrowArgumentException_WhenNullOrEmpty(string? input)
    {
        Action act = () => CaseConventionUtils.GetCaseType(input);
        act.Should().Throw<ArgumentException>();
    }
  
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToSnakeCase_ShouldThrowArgumentException_WhenNullOrEmpty(string? input)
    {
        Action act = () => CaseConventionUtils.ToSnakeCase(input);
        act.Should().Throw<ArgumentException>();
    }
 

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToKebabCase_ShouldThrowArgumentException_WhenNullOrEmpty(string? input)
    {
        Action act = () => CaseConventionUtils.ToKebabCase(input);
        act.Should().Throw<ArgumentException>();
    }
      
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToPascalCase_ShouldThrowArgumentException_WhenNullOrEmpty(string? input)
    {
        Action act = () => CaseConventionUtils.ToPascalCase(input);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToCamelCase_ShouldThrowArgumentException_WhenNullOrEmpty(string? input)
    {
        Action act = () => CaseConventionUtils.ToCamelCase(input);
        act.Should().Throw<ArgumentException>();
    }
     
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToUpperCase_ShouldThrowArgumentException_WhenNullOrEmpty(string? input)
    {
        Action act = () => CaseConventionUtils.ToUpperCase(input);
        act.Should().Throw<ArgumentException>();
    }
}
