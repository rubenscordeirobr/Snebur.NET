using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Snebur.Core;

public interface IResultValue
{
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    bool IsSuccess { get; }
    Error? Error { get; }

    [DebuggerDisplay("Type = {Value?.GetType().Name}, Value = {Value}")]
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    object? Value { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    bool IsFailure => !IsSuccess;
}
