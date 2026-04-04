namespace Snebur.SharedKernel.Abstractions;

public interface IJsonStringLocalizer
{
    string this[string localizationKey, string defaultValue, params object[] args] { get; }

#pragma warning disable CA1043 
    string this[Enum enumValue] { get; }
#pragma warning restore CA1043

}

public interface IJsonStringLocalizer<in T> : IJsonStringLocalizer
{
    //string this[T enumValue] { get; }
    
}

