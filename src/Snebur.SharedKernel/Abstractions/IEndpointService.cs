namespace Snebur.SharedKernel.Abstractions;

public interface IEndpointService
{
    public string ServiceName { get; }
    public ServiceRole ServiceRole { get; }
}
