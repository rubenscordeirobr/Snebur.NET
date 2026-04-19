namespace Snebur.Core.Exceptions;
public static class InternationalDialingCodeExtensions
{
    public static string GetDialingCode(this InternationalDialingCode code)
    {
        var dialingCodeAttribute = code.GetCustomAttribute<DialingCodeAttribute>();
        if (dialingCodeAttribute is null)
        {
            throw new MissingAttributeException(typeof(DialingCodeAttribute).Name, code.GetType().Name);
        }
        return dialingCodeAttribute.Code;
    }
}
