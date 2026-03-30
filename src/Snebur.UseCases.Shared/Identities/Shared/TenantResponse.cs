namespace Snebur.UseCases.Identities.Shared;

public sealed record TenantResponse : ITenant, IResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string TenantName { get; init; }
    public required string Email { get; init; }
    public required Country Country { get; init; }
    public required Language Language { get; init; }
    public required Currency Currency { get; init; }
    public required BusinessType BusinessType { get; init; }
    public required TenantType TenantType { get; init; }
    public required TenantStatus TenantStatus { get; init; }
    public required TenantState TenantState { get; init; }
    public required FiscalCode FiscalCode { get; init; }
    public required PhoneNumber PhoneNumber { get; init; }
    public required TimeZoneOffset TimeZoneOffset { get; init; }
     
    public required AddressDto? Address { get; init; }
}
