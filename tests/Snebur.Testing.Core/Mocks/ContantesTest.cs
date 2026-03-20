namespace Snebur.Testing.Core.Mocks;

public static class ContantesTest
{
    public const string StringToLongMoreThan100
        = "This is a string for tests that is way too long for the validation to pass. It has more than 100 characters and should fail..";

    public const string StringToLongMoreThan255
        = "This is a string for tests that is way too long for the validation to pass. It has more than 255 characters and should fail." +
          "This is a string for tests that is way too long for the validation to pass. It has more than 255 characters and should fail." +
          "This is a string for tests. It has more than 255 characters and should fail.";
}

