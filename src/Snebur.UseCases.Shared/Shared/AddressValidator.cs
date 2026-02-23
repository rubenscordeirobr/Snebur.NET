namespace Snebur.UseCases.Shared;

public class AddressValidator : AbstractValidator<AddressDto>
{
    public AddressValidator(IJsonStringLocalizer<AddressValidator> localizer)
    {
        Guard.NotNull(localizer);

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage(localizer["Address.StreetRequired", "Street is required."])
            .MaximumLength(ValidationConstants.StreetMaxLength)
            .WithMessage(localizer["Address.StreetTooLong", 
                                   "Street cannot be longer than {MaxLength} characters.", 
                                   ValidationConstants.StreetMaxLength]);

        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage(localizer["Address.NumberRequired", "Number is required."])
            .MaximumLength(ValidationConstants.AddressNumberMaxLength)
            .WithMessage(localizer["Address.NumberTooLong",
                                   "Number cannot be longer than {MaxLength} characters.",
                                   ValidationConstants.AddressNumberMaxLength]);

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage(localizer["Address.ZipCodeRequired", "Zip code is required."])
            .MaximumLength(ValidationConstants.ZipCodeMaxLength)
            .WithMessage(localizer["Address.ZipCodeTooLong", "Zip code cannot be longer than {MaxLength} characters."]);

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage(localizer["Address.CityRequired", "City is required."])
            .MaximumLength(ValidationConstants.CityMaxLength)
            .WithMessage(localizer["Address.CityTooLong", "City cannot be longer than {MaxLength} characters."]);

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage(localizer["Address.StateRequired", "State is required."])
            .MaximumLength(ValidationConstants.AddressStateMaxLength)
            .WithMessage(localizer["Address.StateTooLong", "State cannot be longer than {MaxLength} characters."]);

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage(localizer["Address.CountryRequired", "Country is required."])
            .IsInEnumValue()
            .WithMessage(localizer["Address.InvalidCountry", "Invalid country."]);
    }
}
