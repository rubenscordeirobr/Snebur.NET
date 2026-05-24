using Snebur.Core.Utils;

namespace Snebur.UseCases.Shared;

public static class RuleBuilderExtensions
{
    public static string? GetPropertyName<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        var rule = ReflectionUtils.GetPropertyValue(ruleBuilder, "Rule");
        if (rule is not null)
        {
            return ReflectionUtils.GetPropertyValue(rule, "PropertyName")?.ToString();
        }
        return null;
    }
}

