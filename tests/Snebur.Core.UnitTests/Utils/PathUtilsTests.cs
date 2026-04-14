namespace Snebur.Core.UnitTests.Utils;

public class PathUtilsTests
{

    #region RemoveInvalidPathChars Tests

    [Fact]
    public void RemoveInvalidPathChars_ShouldReturnNull_WhenInputIsNull()
    {
        // Arrange
        string? input = null;

        // Act
        string? result = PathUtils.RemoveInvalidPathChars(input);

        // Assert
        result.Should().BeNull("because if input is null, the method should return null");
    }

    [Fact]
    public void RemoveInvalidPathChars_ShouldRemoveAllInvalidCharacters()
    {
        // Arrange
        // Get a list of invalid characters for the current platform.
        char[] invalidChars = Path.GetInvalidPathChars();

        // Create an input that contains a few valid parts and one known invalid character.
        // Here we use the first invalid char to inject in between valid strings.
        char invalidChar = invalidChars.FirstOrDefault();
        string validStart = "validPath";
        string validEnd = "End";
        string input = $"{validStart}{invalidChar}{validEnd}";

        // The expected output should have the invalid character removed.
        string expected = $"{validStart}{validEnd}";

        // Act
        string? result = PathUtils.RemoveInvalidPathChars(input);

        // Assert
        result.Should().Be(expected, "because the invalid character should be removed from the input string");
    }

    [Fact]
    public void RemoveInvalidPathChars_ShouldPreserveInputWithoutInvalidChars()
    {
        // Arrange
        string input = "ThisIsAValidPathString";

        // Act
        string? result = PathUtils.RemoveInvalidPathChars(input);

        // Assert
        result.Should().Be(input, "because the input does not contain any invalid path characters");
    }

    #endregion

    #region RemoveExtension Tests

    [Fact]
    public void RemoveExtension_ShouldReturnNull_WhenInputIsNull()
    {
        // Arrange
        string? input = null;

        // Act
        string? result = PathUtils.RemoveExtension(input);

        // Assert
        result.Should().BeNull("because if input is null, the method should return null");
    }

    [Fact]
    public void RemoveExtension_ShouldReturnFileName_WhenNoDirectoryExists()
    {
        // Arrange
        string input = "file.txt";
        string expected = Path.GetFileNameWithoutExtension(input);

        // Act
        string? result = PathUtils.RemoveExtension(input);

        // Assert
        result.Should().Be(expected, "because if there is no directory, the method should return the file name without extension");
    }

    [Fact]
    public void RemoveExtension_ShouldReturnCombinedPath_WhenDirectoryExists()
    {
        // Arrange
        // Create a path with a directory and an extension.
        // Use Path.Combine to construct the expected output, which makes the test a bit more robust to platform differences.
        string directory = Path.Combine("C:", "folder");
        string fileWithExt = "file.txt";
        string input = Path.Combine(directory, fileWithExt);

        // Calculate expected output.
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(input);
        string? directoryName = Path.GetDirectoryName(input);
        string expected = directoryName is null ? fileNameWithoutExt : Path.Combine(directoryName, fileNameWithoutExt);

        // Act
        string? result = PathUtils.RemoveExtension(input);

        // Assert
        result.Should().Be(expected, "because the method should remove the extension and combine the directory with the file name");
    }

    #endregion
}
