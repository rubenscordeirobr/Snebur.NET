namespace Snebur.Testing.Core.Extensions;

public static class ResultExtensions
{
    public static void ShouldBeSuccessful<TResponse>(
        this Result<TResponse> result)
        where TResponse : notnull
    {
        Guard.NotNull(result);

        result.IsSuccess
            .Should()
            .BeTrue($"Should be successful, but get error {result.Error?.Message}");

        result.IsFailure
            .Should()
            .BeFalse($"Should not be failure, but get error {result.Error?.Message}");

        result.Error
            .Should()
            .BeNull($"Should not have error, but get error {result.Error?.Message}");

        result.Value
            .Should()
            .NotBeNull($"Should have value, but get error {result.Error?.Message}");

        result.Value
            .Should()
            .BeOfType<TResponse>();
    }

    public static void ShouldBeFailureForErrors<TError1, TError2>(
        this IResultValue result,
        bool isAssignableTo = false)
        where TError1 : Error
        where TError2 : Error
    {
        Guard.NotNull(result);

        result.IsFailure
            .Should()
            .BeTrue($"Should be failure, but get value {result.Value}");

        result.IsSuccess
            .Should()
            .BeFalse($"Should not be successful, but get value {result.Value}");

        result.Error
            .Should()
            .NotBeNull($"Should have error, but get value {result.Value}");

        result.Error.Should().Match<Error>(
            e => e is TError1 || e is TError2,
            $"Expected error to be of type {typeof(TError1).Name} or {typeof(TError2).Name}, but got {result.Error?.GetType().Name}");
 
    }

    public static void ShouldBeFailure<TError>(
        this IResultValue result,
        bool isAssignableTo = false)
        where TError : Error
    {
        Guard.NotNull(result);

        result.IsFailure
            .Should()
            .BeTrue($"Should be failure, but get value {result.Value}");

        result.IsSuccess
            .Should()
            .BeFalse($"Should not be successful, but get value {result.Value}");

        result.Error
            .Should()
            .NotBeNull($"Should have error, but get value {result.Value}");

        if (isAssignableTo)
        {
            result.Error
            .Should()
            .BeAssignableTo<TError>($"Should be assignableTo {typeof(TError).Name} error, but get {result.Error?.GetType().Name}");
        }
        else
        {
            result.Error
            .Should()
            .BeOfType<TError>($"Should be {typeof(TError).Name} error, but get {result.Error?.GetType().Name}");
        }
        
    }
}

