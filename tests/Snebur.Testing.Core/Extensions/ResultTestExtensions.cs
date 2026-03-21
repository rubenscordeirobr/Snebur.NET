using System.Linq.Expressions;
using Snebur.Core.Factories;

namespace Snebur.Testing.Core.Extensions;

public static class ResultTestExtensions
{
    public static void ShouldBeSuccessful<T>(
        this ResultTest<T> result)
    {
        Guard.NotNull(result);

        result.Error.Should()
            .BeNull();

        result.IsFailure
            .Should()
            .BeFalse();

        result.IsSuccess
            .Should()
            .BeTrue();
    }

    public static void ShouldBeFailure<T>(
       this ResultTest<T> result)
    {
        Guard.NotNull(result);

        result.Error.Should()
            .NotBeNull();

        result.IsFailure
            .Should()
            .BeTrue();

        result.IsSuccess
            .Should()
            .BeFalse();
    }

    public static void ShouldHaveValidationErrorFor<T>(
        this ResultTest<T> result,
        Expression<Func<T, object>> propertyExpression)
    {
        Guard.NotNull(result);
        Guard.NotNull(propertyExpression);

        result.Error.Should()
            .NotBeNull();

        result.IsFailure
            .Should()
            .BeTrue();

        result.IsSuccess
            .Should()
            .BeFalse();

        var errorCode = ErrorCodeFactory.CreateInvalidCodeFor(propertyExpression);
        result.Error.Should()
            .BeOfType<ValidationError>()
            .Subject.Code
            .Should()
            .Be(errorCode);
    }
}
