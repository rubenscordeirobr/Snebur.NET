using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Snebur.Core;

[DebuggerDisplay("Type = {typeof(T).Name}, Value = {Value}")]
public class Result<T> : IResultValue where T : notnull
{
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailure
        => !IsSuccess;

    public Error? Error { get; }

    [DebuggerDisplay("Type = {Value?.GetType().Name}, Value = {Value}")]
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    [MemberNotNullWhen(false, nameof(IsFailure))]
    public T? Value { get; }

    internal Result(Error error)
    {
        Guard.NotNull(error);

        Error = error;
        IsSuccess = false;
    }

    internal Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    public Result<TConvert> AsFailure<TConvert>() where TConvert : notnull
    {
        if (IsSuccess || Error is null)
            throw new InvalidOperationException("Only failed results can be converted");

        return Result.Failure<TConvert>(Error);
    }

    public T GetRequiredValue()
    {
        if (!IsSuccess || Value is null)
            throw new InvalidOperationException("Cannot get value from failed result");

        return Value;
    }

    public Result<TConvert> ConvertTo<TConvert>()
       where TConvert : notnull
    {
        if (IsSuccess)
        {
            if (Value.GetType().IsAssignableTo(typeof(TConvert)))
            {
                return Result.Success((TConvert)(object)Value!);
            }
            throw new InvalidOperationException($" Cannot convert value {typeof(T).Name} to {typeof(TConvert).Name}");
        }
        return Result.Failure<TConvert>(Error);
    }

    #region IResult
    object? IResultValue.Value => Value;
    #endregion
}

public static class Result
{
    public static Result<T> Success<T>(T value)
        where T : notnull
        => new(value);

    public static Result<T> Failure<T>(Error error)
         where T : notnull
          => new(error);

    public static Result<T> ValidationFailure<T>(
        string code,
        string message)
        where T : notnull
        => new(new ValidationError(code, message));

    public static Result<T> NotFoundFailure<T>(
        string code,
        string message)
        where T : notnull
        => new(new NotFoundError(code, message));
}
