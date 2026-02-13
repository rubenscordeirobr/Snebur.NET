namespace Snebur.Application.Events;

public record ChangedPropertyEvent(
    string PropertyName,
    object? PreviousValue,
    object? Value) : IChangedPropertyEvent
{
    public sealed override string ToString()
        => $"{PropertyName}: {PreviousValue} -> {Value}";
}

public record PropertyValueEvent(
    string PropertyName,
    object? Value) : IPropertyValueEvent;
