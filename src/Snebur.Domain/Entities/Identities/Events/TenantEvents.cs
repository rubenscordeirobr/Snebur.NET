namespace Snebur.Domain.Entities.Identities.Events;

public sealed record TenantCreatedEvent(
   Tenant Tenant,
   TenantUser Owner) : IDomainEvent;

public sealed record TenantUpdatedEvent(
   Tenant Tenant) : IDomainEvent;

public sealed record TenantAddressAddedEvent(
    Tenant tenant,
    TenantAddress newAddress) : IDomainEvent;

public sealed record TenantDefaultAddressUpdatedEvent(
    Tenant tenant,
    TenantAddress? previousAddress,
    TenantAddress address) : IDomainEvent;

public sealed record TenantAddressRemovedEvent(
    Tenant tenant,
    TenantAddress address) : IDomainEvent;
public sealed record TenantTimeZoneOffsetChangedEvent(
    Tenant Tenant,
    TimeZoneOffset PreviousTimeZoneOffset,
    TimeZoneOffset TimeZoneOffset) : IDomainEvent;
