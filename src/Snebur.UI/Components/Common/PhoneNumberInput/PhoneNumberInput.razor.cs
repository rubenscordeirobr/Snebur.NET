using System.Diagnostics.CodeAnalysis;
using Snebur.SharedKernel.Extensions;
using Snebur.SharedKernel.ValueObjects;

namespace Snebur.UI.Components.Common;

public partial class PhoneNumberInput : FluentInputBase<PhoneNumber?>
{
    private InternationalDialingCode _internationalDialingCode;
    private string? _nationalNumber;

    [Inject]
    private ICultureProvider CultureProvider { get; set; }

    public override PhoneNumber? Value
    {
        get => GetPhoneNumber();
        set => SetPhoneNumber(value);
    }
    
    public InternationalDialingCode InternationalDialingCode
    {
        get => _internationalDialingCode;
        set
        {
            if (_internationalDialingCode != value)
            {
                _internationalDialingCode = value;
                NotifyValueChanged();
                StateHasChanged();
            }
        }
    }

    public string? NationalNumber
    {
        get => _nationalNumber;
        set
        {
            if (_nationalNumber != value)
            {
                _nationalNumber = value;
                NotifyValueChanged();
                StateHasChanged();
            }
        }
    } 
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }

    private InternationalDialingCode GetCurrentInternalDialingCode()
    {
        return PhoneNumberUtils.GetInternationalDialingCodeFromCountry(CultureProvider!.GetCountry());
    }

    private PhoneNumber? GetPhoneNumber()
    {
        if (InternationalDialingCode == InternationalDialingCode.Unknown)
            return null;

        return new PhoneNumber(InternationalDialingCode, NationalNumber ?? String.Empty);
    }

    private void SetPhoneNumber(PhoneNumber? value)
    {
        InternationalDialingCode = value?.InternationalDialingCode ?? GetCurrentInternalDialingCode();
        NationalNumber = value?.NationalNumber;
    }
     
    private void NotifyValueChanged()
    {
        ValueChanged.InvokeAsync(Value);
    }

    protected override bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out PhoneNumber? result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        var phoneNumberResult = PhoneNumber.Create(value);
        if (phoneNumberResult.IsFailure)
        {
            validationErrorMessage = phoneNumberResult.Error.Message;
            result = null;
            return false;
        }
        result = phoneNumberResult.Value;
        validationErrorMessage = null;
        return true;
    }
}
