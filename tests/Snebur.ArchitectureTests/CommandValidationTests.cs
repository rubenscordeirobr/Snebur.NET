namespace Snebur.ArchitectureTests;

public class CommandValidationTests
    : IClassFixture<ApplicationAssemblyContext>, 
    IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IReadOnlyDictionary<Type, IReadOnlyList<Type>> _validatorMappings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITestOutputHelper _output;

    public CommandValidationTests(
        ApplicationAssemblyContext assemblyContext,
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper output)
    {
        serviceProviderMock.AddTestOutput(output);

        _validatorMappings = assemblyContext.CommandTypeToValidatorTypeMappings;
        _serviceProvider = serviceProviderMock;
        _output = output;
    }
     
    public static IEnumerable<object[]> CommandTypesData
    {
        get
        {
            var assemblyContext = new ApplicationAssemblyContext();
            var commandTypes = assemblyContext.CommandTypes;
            return commandTypes.Select(type => new object[] { type });
        }
    }
     
    [Theory]
    [MemberData(nameof(CommandTypesData))]
    public void Command_ShouldHaveValidator(Type commandType)
    {
        //Arrange
        var validatorsType = _validatorMappings.GetValueOrDefault(commandType);

        //Assert
        validatorsType.Should()
            .NotBeNull($"The command {commandType.Name} does not have a validator");

        validatorsType.Should()
            .NotBeEmpty($"The command {commandType.Name} does not have a validator");

        validatorsType.Should()
            .HaveCount(1, $"The command {commandType.Name} has more than one validator");

        _output.WriteLine($"Command {commandType.Name} has validator {validatorsType![0].Name}");
    }
     
    [Theory]
    [MemberData(nameof(CommandTypesData))]
    public void CommandValidator_ShouldEnforceStringPropertyValidations(
       Type commandType )
    {
        //Arrange
        var validatorType = typeof(IValidator<>).MakeGenericType(commandType);
        var validator = _serviceProvider.GetService(validatorType) as IValidator;
         
        validator.Should()
            .NotBeNull($"The command {commandType.Name} does not have a validator registered");

        Guard.NotNull(validator);

        var descriptor = validator.CreateDescriptor();
        var stringProperties = commandType.GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        var nullabilityContext = new NullabilityInfoContext();

        foreach (var stringProperty in stringProperties)
        {
            var validators = descriptor
                .GetValidatorsForMember(stringProperty.Name)
                .Select(x => x.Validator);

            var maxLengthValidation = validators
                .FirstOrDefault(validator => validator.GetType().ImplementsGenericInterfaceDefinition(typeof(MaximumLengthValidator<>)));

            maxLengthValidation.Should()
                .NotBeNull($"The property {stringProperty.GetPropertyPath()} does not have a MaxLength validation");

            var nullabilityInfo = nullabilityContext.Create(stringProperty);
            if (nullabilityInfo.ReadState == NullabilityState.Nullable)
            {
                continue;
            }

            var notEmptyValidation = validators
                .FirstOrDefault(x => x.GetType().ImplementsGenericInterfaceDefinition(typeof(NotEmptyValidator<,>)));

            notEmptyValidation.Should()
                .NotBeNull($"The property {stringProperty.GetPropertyPath()} does not have a NotEmpty validation");
        }

        _output.WriteLine($"Command {commandType.Name} has all string properties validated");
    }
}
