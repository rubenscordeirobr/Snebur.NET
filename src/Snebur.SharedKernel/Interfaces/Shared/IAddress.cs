namespace Snebur.SharedKernel.Interfaces.Shared;
public interface IAddress
{
    string Street { get; }
    string Number { get; }
    string? Complement { get; }
    string Neighborhood { get; }
    string City { get; }
    string State { get; }
    string ZipCode { get; }
    Country Country { get; }
}
