using Snebur.SharedKernel.Enums;
using Snebur.SharedKernel.ValueObjects;
using Snebur.UseCases.Identities.Tenants.Commands;

namespace Snebur.FunctionalTests.Identities;

public partial class TenantServiceTests
{
    [Fact]
    public async Task CreateTenant_ShouldBeSuccessful()
    {
        // Arrange
        var fakePhoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var fakeEmail = FakeUtils.GenerateFakeEmail();
        var fakeCpf = BrazilianFakeUtils.GenerateCpf();

        var createCommand = new CreateTenantAccountCommand
        {
            Name = "Tenant name",
            FiscalCode = new FiscalCode(fakeCpf),
            BusinessName = "Tenant name",
            Email = fakeEmail,
            Password = "Password123!",
            IsPersistent= false,
            Country = Country.Brazil,
            Language = Language.PortugueseBrazil,
            Currency = Currency.BRL,
            BusinessType = BusinessType.CivilRegistryOffice,
            TenantType = TenantType.Company,
            PhoneNumber = new PhoneNumber(fakePhoneNumber)
        };

        // Act
        var result = await _clientService.CreateAsync(
            createCommand,
            TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
        result.Value!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateTenant_ShouldBeFailure()
    {
        // Arrange
        var fakePhoneNumber = BrazilianFakeUtils.GenerateFakePhoneNumber();
        var fakeCpf = BrazilianFakeUtils.GenerateCpf();

        var createCommand = new CreateTenantAccountCommand
        {
            Name = "Tenant name",
            FiscalCode = new FiscalCode(fakeCpf),
            BusinessName = "Tenant name",
            Email = string.Empty,
            Password = "Password123!",
            IsPersistent= false,
            Country = Country.Brazil,
            Language = Language.PortugueseBrazil,
            Currency = Currency.BRL,
            BusinessType = BusinessType.CivilRegistryOffice,
            TenantType = TenantType.Company,
            PhoneNumber = new PhoneNumber(fakePhoneNumber)
        };

        // Act
        var result = await _clientService.CreateAsync(createCommand, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFailure<ValidationError>();
    }
}

