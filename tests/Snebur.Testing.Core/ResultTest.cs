namespace Snebur.Testing.Core;

// Existing code
public class ResultTest<T>
{
    private Result<IResponse> _result { get; }

    public Error? Error => _result.Error;

    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Value))]
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => _result.IsSuccess;

    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => _result.IsFailure;

    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(IsSuccess))]
    public IResponse? Value => _result.Value;

    public ResultTest(Result<IResponse> result)
    {
        _result = result;
    }
}
