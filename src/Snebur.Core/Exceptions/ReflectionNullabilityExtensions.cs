using System.Collections.Concurrent;
using System.Reflection;

namespace Snebur.Core.Exceptions;

public static class ReflectionNullabilityExtensions
{
    private static readonly NullabilityInfoContext _context = new();
    private static readonly ConcurrentDictionary<object, NullabilityState> _cache = new();

    public static bool IsNullable(this PropertyInfo property)
    {
        Guard.NotNull(property);

        if (property.PropertyType.IsValueType)
            return false;

        return GetNullabilityState(property) == NullabilityState.Nullable;
    }

    public static bool IsNullable(this ParameterInfo parameter)
    {
        Guard.NotNull(parameter);

        if (parameter.ParameterType.IsValueType)
            return false;

        return GetNullabilityState(parameter) == NullabilityState.Nullable;
    }

    public static bool IsNullable(FieldInfo field)
    {
        Guard.NotNull(field);

        if (field.FieldType.IsValueType)
            return false;

        return GetNullabilityState(field) == NullabilityState.Nullable;
    }

    public static bool IsNullable(this EventInfo eventInfo)
    {
        Guard.NotNull(eventInfo);

        var handlerType = eventInfo.EventHandlerType;
        if (handlerType is null)
            return true;

        if (handlerType.IsValueType)
            return false;

        return GetNullabilityState(eventInfo) == NullabilityState.Nullable;
    }

    private static NullabilityState GetNullabilityState(object memberInfo)
    {
        return _cache.GetOrAdd(memberInfo, CreateNullabilityState);
    }

    private static NullabilityState CreateNullabilityState(object member)
    {
        return member switch
        {
            PropertyInfo p => _context.Create(p).ReadState,
            ParameterInfo p => _context.Create(p).ReadState,
            FieldInfo f => _context.Create(f).ReadState,
            EventInfo e => _context.Create(e).ReadState,
            _ => throw new NotSupportedException(
                $"Unsupported member type: {member.GetType().Name}")
        };
    }
}
