namespace Snebur.SharedKernel.Abstractions;

public interface ICultureProvider
{
    Culture Culture { get; }
    Language Language { get; }
}
