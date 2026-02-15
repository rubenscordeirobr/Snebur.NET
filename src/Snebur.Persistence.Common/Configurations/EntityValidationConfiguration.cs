using Snebur.Persistence.Common.Exceptions;
using Snebur.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Snebur.Persistence.Common.Configurations;

public static class EntityValidationConfiguration
{
    private const string DiscriminatorPropertyName = "Discriminator";

    internal static void Validate<TContext>(
        this IMutableEntityType mutableEntityType )
        where TContext : DbContext
    {
        foreach (var mutableProperty in mutableEntityType.GetProperties())
        {
            mutableProperty
                .CheckMaxLength()
                .CheckEnums<TContext>();
        }
    }

    private static IMutableProperty CheckMaxLength(
       this IMutableProperty mutableProperty )
    {
        if (!mutableProperty.IsPropertyString())
        {
            return mutableProperty;
        }

        if (mutableProperty.Name != DiscriminatorPropertyName &&
            mutableProperty.GetMaxLength() is null)
        {
            var declaringTypeQualifiedName = mutableProperty.PropertyInfo?.DeclaringType?.GetQualifiedName() ?? "Unknown";

            var errorMessage = $"Entity {declaringTypeQualifiedName} property {mutableProperty.Name} must have a max length defined" +
                              $"Set the max length in EntityBuilderConfiguration using HasMaxLength method";
           
            throw new MaxLengthNotDefinedException(errorMessage);
      
        }
        return mutableProperty;
    }

    private static bool IsPropertyString(this IMutableProperty mutableProperty)
    {
        if (mutableProperty.ClrType != typeof(string))
        {
            return false;
        }

        var declaringType = mutableProperty.PropertyInfo?.DeclaringType;
        if (declaringType == null)
        {
            return false;
        }

        var isDeclaredInValueObject = declaringType
            .IsSubclassOf<ValueObjectBase>();

        if (isDeclaredInValueObject)
        {
            return false;
        }
        return true;
    }
    }

