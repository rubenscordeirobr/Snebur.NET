using Snebur.Core.Utils;
using FluentValidation.Results;

namespace Snebur.UseCases.Shared;
public static partial class DefaultValidationsExtensions
{
    public static IRuleBuilderOptions<T, PhoneNumber> PhoneNumber<T>(
        this IRuleBuilder<T, PhoneNumber> ruleBuilder,
        IJsonStringLocalizer localizer)
    {
        Guard.NotNull(ruleBuilder);

        return ruleBuilder
            .SetValidator(new PhoneNumberValidator(localizer, ruleBuilder.GetPropertyName()));
    }

    public static IRuleBuilderOptions<T, string> PhoneNumber<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IJsonStringLocalizer localizer)
    {
        Guard.NotNull(ruleBuilder);
        Guard.NotNull(localizer);

        return ruleBuilder
            .NotEmpty()
            .WithMessage(localizer["PhoneNumber.Number", "Phone number cannot be empty."])
            .MaximumLength(ValidationConstants.PhoneNumberMaxLength)
            .WithMessage(localizer["PhoneNumber.TooLong", "Phone number cannot be longer than {MaxLength} characters."])
            .Must(IsFullPhoneNumberValid);
    }

    private static bool IsFullPhoneNumberValid(string fullNumber)
    {
        return PhoneNumberValidationUtils.IsFullPhoneNumberValid(fullNumber);
    }
}

public class PhoneNumberValidator : AbstractValidator<PhoneNumber>
{
    private readonly string? _propertyName;
    public PhoneNumberValidator(
        IJsonStringLocalizer localizer,
        string? propertyName)
    {
        _propertyName = propertyName;
        Guard.NotNull(localizer);

        RuleFor(x => x.InternationalDialingCode)
            .IsInEnumValue()
            .WithMessage(localizer["PhoneNumber.DialingCodeInvalid", "Invalid dialing code."]);

        RuleFor(x => x.FullNumber)
            .NotEmpty()
            .WithMessage(localizer["PhoneNumber.NumberInvalid", "Phone number cannot be empty."])
            .MaximumLength(ValidationConstants.PhoneNumberMaxLength)
            .WithMessage(localizer["PhoneNumber.TooLong", "Phone number cannot be longer than {MaxLength} characters."])
            .Must(IsFullPhoneNumberValid)
            .WithMessage(localizer["PhoneNumber.InvalidNumberInvalid", "Invalid phone number."]);

        RuleFor(x => x.AreaCode)
          .Must(IsAreaCodeValid)
          .WithMessage(localizer["PhoneNumber.AreaCodeInvalid", "Invalid area code"]);
    }
     
    private bool IsFullPhoneNumberValid(string number)
    {
        return PhoneNumberValidationUtils.IsFullPhoneNumberValid(number);
    }

    private bool IsAreaCodeValid(PhoneNumber phoneNumber, string areaCode)
    {
        return PhoneNumberValidationUtils.IsValidAreaCode(phoneNumber.Country, areaCode);
    }

    public override ValidationResult Validate(ValidationContext<PhoneNumber> context)
    {
        Guard.NotNull(context);

        if (context.InstanceToValidate is null)
        {
            return new ValidationResult([
                new ValidationFailure(context.PropertyPath, "Phone number cannot be null.")
            ]);
        }
        return base.Validate(context);
    }

    public override async Task<ValidationResult> ValidateAsync(
        ValidationContext<PhoneNumber> context, 
        CancellationToken cancellation = default)
    {
        var result = await base.ValidateAsync(context, cancellation);
        if (string.IsNullOrWhiteSpace(_propertyName))
        {
            return result;
        }

        foreach (var error in result.Errors)
        {
            error.PropertyName = _propertyName;
        }
        return result;
    }
}
