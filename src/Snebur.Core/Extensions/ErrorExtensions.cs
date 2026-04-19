namespace Snebur.Core.Extensions;

public static class ErrorExtensions
{
    public static string GetErrorCode<T>(this object _, string memberName)
    {
        return ErrorCodeFactory.CreateCodeFor<T>(memberName);
    }

    public static string GetErrorCode(this object obj, string memberName)
    {
        Guard.NotNull(obj);
        return ErrorCodeFactory.CreateCodeFor(obj.GetType(), memberName);
    }
}

