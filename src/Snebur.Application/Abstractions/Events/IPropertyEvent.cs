namespace Snebur.Application.Abstractions.Events;

public interface IPropertyEvent
{
    string PropertyName { get; }
}

public interface IPropertyValueEvent: IPropertyEvent
{
    object? Value { get; }
}

public interface IChangedPropertyEvent : IPropertyEvent
{
    public object? PreviousValue { get; }
    public object? Value { get; }
}

