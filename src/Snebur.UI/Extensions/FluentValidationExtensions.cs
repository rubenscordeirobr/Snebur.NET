using FluentValidation;

namespace Snebur.UI.Extensions;

public static class FluentValidationExtensions
{
    public static void ValidateWithCommand<T, TCommand>(
        this IRuleBuilder<T, T> ruleBuilder,
        Func<T, TCommand> commandFactory,
        IValidator<TCommand> validator)
    {
       ruleBuilder.CustomAsync(async (model, context, cancellation) =>
      {
          var result = await validator.ValidateAsync(commandFactory(model), cancellation);
          if (!result.IsValid)
          {
              foreach (var error in result.Errors)
              {
                  context.AddFailure(error.PropertyName, error.ErrorMessage);
              }
          }
      });
    }
}
