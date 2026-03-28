using Snebur.Application.Exceptions;
using Snebur.Core.Factories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Snebur.RuntimeServices.Mediators;

public class CommandValidatorExecutor<TResponse>
    where TResponse : IResponse
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICommandRequest<TResponse> _command;
    private readonly ILogger _logger;

    public CommandValidatorExecutor(
        IServiceProvider serviceProvider,
        ILogger logger,
        ICommandRequest<TResponse> request )
    {
        _serviceProvider = serviceProvider;
        _command = request;
        _logger = logger;
    }

    public async Task<Result<bool>> ValidateAsync(CancellationToken cancellationToken)
    {
        var commandValidatorType = typeof(IValidator<>).MakeGenericType(_command.GetType());

        var commandValidator = _serviceProvider.GetRequiredService(commandValidatorType) as IValidator;

        if(commandValidator is null)
        {
            var commandTypeName = _command.GetType().GetQualifiedName();
             
            _logger.LogError("Command Validator not found for command {CommandTypeName}", commandTypeName);

            var errorMessage = $"Command Validator not found for command {commandTypeName}";
            var exception = new CommandValidatorNotFoundException(
                errorMessage);
          
            return Result.Failure<bool>(new CommandValidatorNotFoundError(
                exception,
                Code: ErrorCodeFactory.CommandValidatorFoundCodeFor(commandTypeName),
                Message: errorMessage));
        }
             
        var validationContext = CreateValidationContext();
        var result = await commandValidator.ValidateAsync(validationContext, cancellationToken);

        if (result.IsValid)
        {
            return Result.Success(true);
        }

        var commandType = _command.GetType();
        var validationFailure = result.Errors.FirstOrDefault() 
            ?? throw new InvalidOperationException($"Validation Error not found for command {_command.GetType()}");

        var errorCode = ErrorCodeFactory.CreateInvalidCodeFor(
            commandType, 
            validationFailure.PropertyName);

        var error = new ValidationError(
            Code: errorCode,
            Message: validationFailure.ErrorMessage);

        return Result.Failure<bool>(error);
    }

    private IValidationContext CreateValidationContext()
    {
        var type = typeof(ValidationContext<>).MakeGenericType(_command.GetType());
        var instance = Activator.CreateInstance(type, _command);

        if (instance is null)
            throw new InvalidOperationException($"Validation Context not created for command {_command.GetType()}");

        return (IValidationContext)instance;
    }
}
