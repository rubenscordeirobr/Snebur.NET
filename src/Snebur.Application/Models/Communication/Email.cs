namespace Snebur.Application.Models.Communication;

public record Email
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public required string From { get; init; }
    public string? FromName { get; init; }

    
}
