namespace Snebur.ArchitectureTests.TestSupport ;

public class UnitTestTypeNamesCollection
{
    private HashSet<string> _unitTestTypes { get; }

    public UnitTestTypeNamesCollection()
    {
        var applicationTestsAssembly = typeof(Application.UnitTests.Domain.Extensions.EntityDeletedExtensionsTests).Assembly;
        var useCaseTestsAssembly = typeof(UseCases.UnitTests.Activities.Events.EntityCreatedEventHandlerTests).Assembly;
        var infrastructureTestsAssembly = typeof(Infrastructure.UnitTests.EmailSenderTests).Assembly;
        var identityFunctionalTestsAssembly = typeof(FunctionalTests.HttpClientTests).Assembly;
       
        var allTypes = applicationTestsAssembly.GetTypes()
            .Concat(infrastructureTestsAssembly.GetTypes())
            .Concat(useCaseTestsAssembly.GetTypes())
            .Concat(identityFunctionalTestsAssembly.GetTypes());

        _unitTestTypes = [.. allTypes
            .Where(type => type.Name.EndsWith("Tests"))
            .Select(type => type.Name)];
    }

    public int Count
        => _unitTestTypes.Count;

    public bool Contains(string item)
        => _unitTestTypes.Contains(item);

}

