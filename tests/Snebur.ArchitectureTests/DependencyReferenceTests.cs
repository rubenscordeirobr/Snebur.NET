using NetArchTest.Rules;

namespace Snebur.ArchitectureTests;

public class DependencyReferenceTests : IClassFixture<ApplicationAssemblyContext>
{
    private readonly ApplicationAssemblyContext _context;
    public DependencyReferenceTests(ApplicationAssemblyContext assemblyContext)
    {
        _context = assemblyContext;
    }

    [Fact]
    public void CommonAssembly_ShouldNotHaveDependencies()
    {
        //Arrange
        string[] dependencies = [
            _context.DomainAssemblyName,
            _context.ApplicationAssemblyName,
            _context.UseCasesSharedAssemblyName,
            _context.UseCasesAssemblyName,
            _context.PresentationAssemblyName,
            _context.ClientGatewayAssemblyName,
             .._context.InfrastructuresAssemblyNames];

        //Act
        var result = Types.InAssembly(_context.CommonAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(dependencies)
            .GetResult();

        //Assert
        result.IsSuccessful
                .Should()
                .BeTrue($"The CommonAssembly should not have dependencies on Domain, " +
                        $"Application, UseCases, UseCasesShared, Presentation and Infrastructure layers.\r\n" +
                        $"FailingTypeNames {result.GetFailingTypeNames()}");
    }

    [Fact]
    public void UseSharedKernelAssembly_ShouldNotHaveDependencies()
    {
        //Arrange
        string[] dependencies = [
            _context.DomainAssemblyName,
            _context.ApplicationAssemblyName,
            _context.UseCasesSharedAssemblyName,
            _context.PresentationAssemblyName,
            .._context.InfrastructuresAssemblyNames];

        //Act
        var result = Types.InAssembly(_context.SharedKernelAssembly)
               .ShouldNot()
               .HaveDependencyOnAny(dependencies)
               .GetResult();

        //Assert
        result.IsSuccessful
            .Should()
            .BeTrue($"The SharedKernelAssembly should not have dependencies on Domain, " +
                    $"Application, UseCasesShared, Presentation and Infrastructure layers.\r\n" +
                    $"FailingTypeNames {result.GetFailingTypeNames()}");
    }

    [Fact]
    public void ApplicationAssembly_ShouldNotHaveInfrastructureDependencies()
    {
        //Arrange
        string[] dependencies = [
            _context.UseCasesAssemblyName,
            .._context.InfrastructuresAssemblyNames];

        //Act
        var result = Types.InAssembly(_context.ApplicationAssembly)
             .ShouldNot()
             .HaveDependencyOnAny(dependencies)
             .GetResult();

        //Assert
        result.IsSuccessful
            .Should()
            .BeTrue($"The ApplicationAssembly should not have dependencies on UseCases and Infrastructure layers.\r\n" +
                    $"FailingTypeNames {result.GetFailingTypeNames()}");
    }

    [Fact]
    public void UserCasesAssembly_ShouldNotHaveDependencies()
    {
        //Arrange
        string[] dependencies = _context.InfrastructuresAssemblyNames;

        //Act
        var result = Types.InAssembly(_context.UseCasesAssembly)
              .ShouldNot()
              .HaveDependencyOnAny(dependencies)
              .GetResult();

        //Assert
        result.IsSuccessful
            .Should()
            .BeTrue($"The UseCasesAssembly should not have dependencies on Infrastructure layers.\r\n" +
                    $"FailingTypeNames {result.GetFailingTypeNames()}");
    }

    [Fact]
    public void UseCasesSharedAssembly_ShouldNotHaveDomainAndInfrastructureDependencies()
    {
        //Arrange
        string[] dependencies = [
           _context.DomainAssemblyName,
           _context.ApplicationAssemblyName,
           _context.PresentationAssemblyName,
           .._context.InfrastructuresAssemblyNames
        ];

        //Act
        var result = Types.InAssembly(_context.UseCasesSharedAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(dependencies)
            .GetResult();

        //Assert
        result.IsSuccessful
                .Should()
                .BeTrue($"The UseCasesSharedAssembly should not have dependencies on Domain, " +
                        $"Application, UseCases and Infrastructure layers.\r\n" +
                        $"FailingTypeNames {result.GetFailingTypeNames()}");
    }

    [Fact]
    public void PresentationAssembly_ShouldNotHaveDomainDependencies()
    {
        //Arrange
        var domainAssemblyName = _context.DomainAssemblyName;

        //Act
        var result = Types.InAssembly(_context.PresentationAssembly)
            .ShouldNot()
            .HaveDependencyOn(domainAssemblyName)
            .GetResult();

        //Assert
        result.IsSuccessful
               .Should()
               .BeTrue(because: "Presentation layer should not have dependencies on Domain layer." +
                                $" FailingTypeNames {result.GetFailingTypeNames()}");
    }

    [Fact]
    public void ClientGatewayAssembly_ShouldNotHaveDomainDependencies()
    {
        //Arrange
        string[] dependencies = [
           _context.DomainAssemblyName,
           _context.ApplicationAssemblyName,
           _context.PresentationAssemblyName,
           .._context.InfrastructuresAssemblyNames,
        ];

        //Act
        var result = Types.InAssembly(_context.ClientGatewayAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(dependencies)
            .GetResult();

        //Assert
        result.IsSuccessful
                .Should()
                .BeTrue($"The ClientGatewayAssembly should not have dependencies on Domain, " +
                        $"Application, UseCases and Infrastructure layers.\r\n" +
                        $"FailingTypeNames {result.GetFailingTypeNames()}");
    }
}
