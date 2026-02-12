using System.Data;
using System.Text.Json.Serialization;
using Snebur.Core.Infos;

namespace Snebur.SharedKernel.ValueObjects;

public record PhoneNumber : ValueObjectBase
{
    [JsonIgnore]
    private readonly PhoneNumberInfo _phoneNumberInfo;

    [JsonIgnore]
    public Country Country
        => _phoneNumberInfo.Country;

    [JsonIgnore]

    public InternationalDialingCode InternationalDialingCode
        => _phoneNumberInfo.InternationalDialingCode;

    [JsonIgnore]

    public string NationalNumber
        => _phoneNumberInfo.NationalNumber;

    [JsonIgnore]
    public string AreaCode
        => _phoneNumberInfo.AreaCode;

    public string FullNumber
         => _phoneNumberInfo.FullNumber;

    public PhoneNumber()
    {
        _phoneNumberInfo = PhoneNumberInfo.Unknown(string.Empty);
    }

    [JsonConstructor]
    public PhoneNumber(string fullNumber)
    {
        _phoneNumberInfo = PhoneNumberInfoParser.Parse(fullNumber);
    }

    public PhoneNumber(InternationalDialingCode dialingCode, string nationalNumber)
    {
        _phoneNumberInfo = PhoneNumberInfoParser.Parse(dialingCode, nationalNumber);
    }

    private PhoneNumber(PhoneNumberInfo phoneNumberInfo)
    {
        _phoneNumberInfo = phoneNumberInfo;
    }

    public static Result<PhoneNumber> Create(string? number)
    {
        var result = PhoneNumberInfo.Create(number);
        if (result.IsFailure)
        {
            return result.AsFailure<PhoneNumber>();
        }
        return Result.Success(new PhoneNumber(result.Value!));
    }

    public sealed override string ToString()
    {
        return FullNumber;
    }

    public override int GetHashCode()
    {
        return FullNumber.GetHashCode();
    }

#pragma warning disable CA2225 

    public static implicit operator PhoneNumber(string value) => new(value);

#pragma warning restore CA2225 

}
