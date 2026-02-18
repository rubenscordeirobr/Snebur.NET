using Snebur.Core.Exceptions;
using Snebur.Core.Utils;
using FluentValidation.Validators;

namespace Snebur.UseCases.Common.Validations;

public static partial class DefaultValidationsExtensions
{
    [Obsolete("Use IsInEnumValue instead.")]
    public static IRuleBuilderOptions<T, TProperty> IsInEnum<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        throw new DeprecatedException("Use IsInEnumValue instead.");
    }

    public static IRuleBuilderOptions<T, TProperty> IsInEnumValue<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder)
        where TProperty : struct, Enum
    {
        Guard.NotNull(ruleBuilder);

        return ruleBuilder
            .SetValidator(new EnumValueValidator<T, TProperty>());
    }
}

public class EnumValueValidator<T, TProperty> : EnumValidator<T, TProperty>
    where TProperty : struct, Enum
{
    public override string Name
        => "EnumValueValidator";

    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        if (!base.IsValid(context, value))
        {
            return false;
        }
        return EnumUtils.IsDefined(value);

    }
}
