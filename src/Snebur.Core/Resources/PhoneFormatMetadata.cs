using Snebur.Core.Infos;

namespace Snebur.Core.Resources;

internal static class PhoneNumberFormatRegistry 
{
    public static PhoneNumberFormatInfo? TryGet(Country countryCode)
    {
        return Mapping.TryGetValue(countryCode, out var info) ? info : null;
    }

    private static readonly Dictionary<Country, PhoneNumberFormatInfo> _mapping = new()
    {
        { Country.UnitedStates,  new PhoneNumberFormatInfo {
            CountryCode = Country.UnitedStates,
            InternationalDialingCode = InternationalDialingCode.UnitedStates,
            InternationalDialingCodeLength = 1,
            MinAreaCodeLength = 3,
            MaxAreaCodeLength = 3,
            MinNationalNumberLength = 10,  // 3 (area) + 7 (subscriber)
            MaxNationalNumberLength = 10,  // Fixed 10-digit length
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(###) ###-####",
        }},
        { Country.Canada, new PhoneNumberFormatInfo {
            CountryCode = Country.Canada,
            InternationalDialingCode = InternationalDialingCode.Canada,
            InternationalDialingCodeLength = 1,
            MinAreaCodeLength = 3,
            MaxAreaCodeLength = 3,
            MinNationalNumberLength = 10,  // 3 (area) + 7 (subscriber)
            MaxNationalNumberLength = 10,  // Fixed 10-digit length
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(###) ###-####",
        }},
        { Country.Mexico,  new PhoneNumberFormatInfo {
            CountryCode = Country.Mexico,
            InternationalDialingCode = InternationalDialingCode.Mexico,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 3,
            MinNationalNumberLength = 10,
            MaxNationalNumberLength = 11,
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(##) #### ####",  // Landline format
            AlternateNationalFormats = [("(###) #### ####", 11)],  // Mobile format
        }},
        { Country.Argentina, new PhoneNumberFormatInfo {
            CountryCode = Country.Argentina,
            InternationalDialingCode = InternationalDialingCode.Argentina,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 4,
            MinNationalNumberLength = 10,  // (2-digit area code) + (8-digit number)
            MaxNationalNumberLength = 13,  // (4-digit area code) + (7-digit number, mobile)
            IsLeadingZeroNeeded = true,  // Leading "0" required for domestic calls
            PrimaryNationalFormat = "0## #### ####",  // Landline format
            AlternateNationalFormats = [("0### 15 #### ####", 13)],  // Mobile format
        }},
        { Country.Bolivia, new PhoneNumberFormatInfo {
            CountryCode = Country.Bolivia,
            InternationalDialingCode = InternationalDialingCode.Bolivia,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 8,  // (2-digit area code) + (6-digit subscriber number)
            MaxNationalNumberLength = 9,  // (2-digit area code) + (7-digit subscriber number)
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(##) ######",  // Landline format
            AlternateNationalFormats = [("(##) #######", 9)]  // Mobile format
        }},
        { Country.Brazil, new PhoneNumberFormatInfo {
            CountryCode = Country.Brazil,
            InternationalDialingCode = InternationalDialingCode.Brazil,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 10,  // (2-digit area code) + (8-digit landline number)
            MaxNationalNumberLength = 11,  // (2-digit area code) + (9-digit mobile number)
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(##) #####-####",  // Mobile format (most common)
            AlternateNationalFormats = [("(##) ####-####", 10) ]  // Landline format
        }},
        { Country.Chile, new PhoneNumberFormatInfo {
            CountryCode = Country.Chile,
            InternationalDialingCode = InternationalDialingCode.Chile,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 9,
            MaxNationalNumberLength = 9,
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(##) #### ####"
        }},
        { Country.Colombia, new PhoneNumberFormatInfo {
            CountryCode = Country.Colombia,
            InternationalDialingCode = InternationalDialingCode.Colombia,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 3,
            MaxAreaCodeLength = 3,
            MinNationalNumberLength = 10,
            MaxNationalNumberLength = 10,
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(###) #### ####"
        }},
        { Country.Ecuador, new PhoneNumberFormatInfo {
            CountryCode = Country.Ecuador,
            InternationalDialingCode = InternationalDialingCode.Ecuador,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 9,
            MaxNationalNumberLength = 9,
            IsLeadingZeroNeeded = true,
            PrimaryNationalFormat = "0## #### ####"
        }},
        { Country.Guyana, new PhoneNumberFormatInfo {
            CountryCode = Country.Guyana,
            InternationalDialingCode = InternationalDialingCode.Guyana,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 7,
            MaxNationalNumberLength = 7,
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(##) #######"
        }},
        { Country.Peru, new PhoneNumberFormatInfo {
            CountryCode = Country.Peru,
            InternationalDialingCode = InternationalDialingCode.Peru,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 1,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 9,
            MaxNationalNumberLength = 9,
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(#) #### ####"
        }},
        { Country.Paraguay, new PhoneNumberFormatInfo {
            CountryCode = Country.Paraguay,
            InternationalDialingCode = InternationalDialingCode.Paraguay,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 9,
            MaxNationalNumberLength = 9,
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(##) #### ####"
        }},
        { Country.Uruguay, new PhoneNumberFormatInfo {
            CountryCode = Country.Uruguay,
            InternationalDialingCode = InternationalDialingCode.Uruguay,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 8,
            MaxNationalNumberLength = 8,
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(##) #### ####"
        }},
        { Country.Suriname, new PhoneNumberFormatInfo {
            CountryCode = Country.Suriname,
            InternationalDialingCode = InternationalDialingCode.Suriname,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 1,
            MaxAreaCodeLength = 1,
            MinNationalNumberLength = 7,  // (1-digit area code) + (6-digit subscriber number)
            MaxNationalNumberLength = 7,  // Fixed length of 7 digits
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(#) ######"
         }},
        { Country.Venezuela, new PhoneNumberFormatInfo {
            CountryCode = Country.Venezuela,
            InternationalDialingCode = InternationalDialingCode.Venezuela,
            InternationalDialingCodeLength = 3,
            MinAreaCodeLength = 3,
            MaxAreaCodeLength = 3,
            MinNationalNumberLength = 10,  // (3-digit area code) + (7-digit subscriber number)
            MaxNationalNumberLength = 10,  // Fixed 10-digit length
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "(###) #######"
        }},
        { Country.Spain, new PhoneNumberFormatInfo {
            CountryCode = Country.Spain,
            InternationalDialingCode = InternationalDialingCode.Spain,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 3,
            MinNationalNumberLength = 9,  // Including area code
            MaxNationalNumberLength = 9,  // Fixed 9-digit length
            IsLeadingZeroNeeded = false,
            PrimaryNationalFormat = "### ### ###"
        }},
        { Country.Germany, new PhoneNumberFormatInfo {
            CountryCode = Country.Germany,
            InternationalDialingCode = InternationalDialingCode.Germany,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 5,  // Germany has variable-length area codes
            MinNationalNumberLength = 10,  // Including area code
            MaxNationalNumberLength = 11,  // Some numbers have 11 digits total
            IsLeadingZeroNeeded = true,
            PrimaryNationalFormat = "0### #######",
            AlternateNationalFormats = [ ( "0## #######", 10), ( "0#### ######", 11) ]
        }},
        { Country.France, new PhoneNumberFormatInfo {
            CountryCode = Country.France,
            InternationalDialingCode = InternationalDialingCode.France,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 1,
            MaxAreaCodeLength = 1,
            MinNationalNumberLength = 10,  // Fixed 10-digit length
            MaxNationalNumberLength = 10,
            IsLeadingZeroNeeded = true,
            PrimaryNationalFormat = "0# ## ## ## ##"
        }},
        { Country.UnitedKingdom, new PhoneNumberFormatInfo {
            CountryCode = Country.UnitedKingdom,
            InternationalDialingCode = InternationalDialingCode.UnitedKingdom,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 5,  // UK has variable-length area codes
            MinNationalNumberLength = 10,  // Including area code
            MaxNationalNumberLength = 11,  // Some mobile numbers have 11 digits
            IsLeadingZeroNeeded = true,
            PrimaryNationalFormat = "0## #### ####",
            AlternateNationalFormats =   [( "0#### ######" , 11), ( "0### ### ####", 10)]
         }},
        { Country.Italy, new PhoneNumberFormatInfo {
            CountryCode = Country.Italy,
            InternationalDialingCode = InternationalDialingCode.Italy,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 4,
            MinNationalNumberLength = 9,  // Including area code
            MaxNationalNumberLength = 11,  // Some mobile numbers have 11 digits
            IsLeadingZeroNeeded = true,
            PrimaryNationalFormat = "0## #### ####",
            AlternateNationalFormats = [ ("0### ### ####", 10), ("0#### ### ###", 10)]
         }},
        { Country.Portugal, new PhoneNumberFormatInfo {
            CountryCode = Country.Portugal,
            InternationalDialingCode = InternationalDialingCode.Portugal,
            InternationalDialingCodeLength = 2,
            MinAreaCodeLength = 2,
            MaxAreaCodeLength = 2,
            MinNationalNumberLength = 9,  // Fixed 9-digit length
            MaxNationalNumberLength = 9,
            IsLeadingZeroNeeded = true,
            PrimaryNationalFormat = "0## ### ####"
        }}
    };

    public static Dictionary<Country, PhoneNumberFormatInfo> Mapping => _mapping;
}
