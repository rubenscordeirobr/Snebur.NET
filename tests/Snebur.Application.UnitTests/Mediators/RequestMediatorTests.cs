using Snebur.Application.Commands;
using Snebur.Application.Exceptions;
using Snebur.SharedKernel.Abstractions;
using Snebur.UseCases;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace Snebur.Application.UnitTests.Mediators;

public class RequestMediatorTests 
{
     
 
    [Fact]
    public async Task RequestMediator_ShouldRunCommandHandler()
    {
        // Arrange
        var mediator = CreateMockRequestMediator();
        var request = new MockCommandRequest { Name = "Test" };

        // Act
        var result = await mediator.RunAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task RequestMediator_ShouldReturnValidationFailure()
    {
        // Arrange
        var mediator = CreateMockRequestMediator();
        var request = new MockCommandRequest { Name = "" };

        // Act
        var result = await mediator.RunAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure
            .Should()
            .BeTrue();

        result.Error
            .Should()
            .NotBeNull();

        result.Error
            .Should()
            .BeOfType<ValidationError>();
    }

    [Fact]
    public void RequestMediator_ShouldThrowCommandValidatorNotFoundException()
    {
        // Arrange
        var mediator = CreateMockRequestMediator();
        var request = new MockValidatorNotRegisteredCommandRequest();

        // Act
        Func<Task> act = async () => await mediator.RunAsync(request, TestContext.Current.CancellationToken);
        // Assert
        act.Should()
            .ThrowAsync<CommandValidatorNotFoundException>();
    }

    [Fact]
    public void RequestMediator_ShouldThrowRequestHandlerNotFoundException()
    {
        // Arrange
        var mediator =CreateMockRequestMediator();
        var request = new MockHandlerNotRegisteredCommandRequest();
        
        // Act
        Func<Task> act = async () => await mediator.RunAsync(request, TestContext.Current.CancellationToken);
        // Assert
        act.Should()
            .ThrowAsync<RequestHandlerNotFoundException>();

    }

    private IRequestMediator CreateMockRequestMediator()
    {
        var serviceProvider = CreateMockServiceProvider();
        var trackingService = CreateMockTrackingService();
        var loogerMock = new Mock<ILogger<RequestMediator>>().Object;
       
        return new RequestMediator(
            serviceProvider,
            trackingService,
            loogerMock);
    }

    private IServiceProvider CreateMockServiceProvider()
    {
        var types = new Type[]
        {
            typeof(MockCommandHandler),
            typeof(MockCommandRequest),
            typeof(MockCommandRequestValidator),
            typeof(MockHandlerNotRegisteredCommandRequest),
            typeof(MockHandlerNotRegisteredCommandRequestValidator),
        };

        return new ServiceCollection()
            .AddApplicationHandlersFromTypes(types)
            .AddCommandValidationServicesFromTypes(types)
            .BuildServiceProvider();
    }

    private ICommandTrackingService CreateMockTrackingService()
    {
        var trackingServiceMock = new Mock<ICommandTrackingService>();

        trackingServiceMock
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        trackingServiceMock.Setup(x =>
            x.TrackAsync(It.IsAny<Guid>(),
            It.IsAny<Result<MockResponse>>())
        );

        return trackingServiceMock.Object;
    }

    public record MockResponse : IResponse
    {
    }

    public record MockCommandRequest : ICommandRequest<MockResponse>
    {
        public Guid ClientRequestId { get; }
        public DateTime? ValidatedSuccessfullyAt { get; set; }
        public required string Name { get; init; }
    }

    public class MockCommandRequestValidator : AbstractValidator<MockCommandRequest>
    {
        public MockCommandRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }

    public class MockCommandHandler : CommandHandler<MockCommandRequest, MockResponse>
    {
        protected override Task<Result<MockResponse>> HandleAsync(
            MockCommandRequest command, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success(new MockResponse()));
        }
    }

    public record MockValidatorNotRegisteredCommandRequest : ICommandRequest<MockResponse>
    {
        public Guid ClientRequestId => Guid.NewGuid();
        public DateTime? ValidatedSuccessfullyAt { get; set; }
    }

    public record MockHandlerNotRegisteredCommandRequest : ICommandRequest<MockResponse>
    {
        public Guid ClientRequestId => Guid.NewGuid();
        public DateTime? ValidatedSuccessfullyAt { get; set; }
    }

    public class MockHandlerNotRegisteredCommandRequestValidator : AbstractValidator<MockHandlerNotRegisteredCommandRequest>
    {
        public MockHandlerNotRegisteredCommandRequestValidator()
        {
            RuleFor(x => x.ClientRequestId)
                .NotEmpty();
        }
    }
}
