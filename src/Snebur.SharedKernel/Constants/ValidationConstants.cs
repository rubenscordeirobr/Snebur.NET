namespace Snebur.SharedKernel.Constants;

public static class ValidationConstants
{
    public const int NameMinLength = 4;
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 255;
    public const int EmailMaxLength = 255;
    public const int PhoneNumberMaxLength = 20;
    public const int UrlMaxLength = 2048;

    // User
    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 50;
    public const int PasswordHashLength = 64;
    
    // Tenant
    public const int CurrencyMaxLength = 3;
    public const int CultureMaxLength = 15; //ca-ES-valencia
    public const int FiscalCodeMaxLength = 20;
    public const int DefaultTimeZoneMaxLength = 50;

    // Address
    public const int StreetMaxLength = 255;
    public const int AddressComplementMaxLength = 100;
    public const int CityMaxLength = 50;
    public const int AddressStateMaxLength = 2;
    public const int CountryMaxLength = 2;
    public const int ZipCodeMaxLength = 10;
    public const int AddressNumberMaxLength = 10;
    public const int NeighborhoodMaxLength = 100;
    public const int AddressNameMaxLength = 50;

    public const int IpAddressMaxLength = 45;
    public const int UserAgentMaxLength = 1000;
}

