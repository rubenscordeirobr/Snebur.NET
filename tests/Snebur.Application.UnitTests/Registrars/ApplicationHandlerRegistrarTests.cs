using Snebur.Application.Commands;
using Snebur.Application.Abstractions.Events;
using Snebur.Application.Abstractions.Handlers;
using Snebur.Application.Exceptions;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.Application.UnitTests.Registrars;

public class ApplicationHandlerRegistrarTests
{
    private Type[] GetHandlerTypes()
    {
        return
        [
            typeof(MockCommandHandler),
            typeof(MockSingleQueryHandler),
            typeof(MockCollectionQueryHandler),
            typeof(MockEventHandler)
        ];
    }

    [Fact]
    public void ApplicationHandlerRegistrar_ShouldRegisterHandlersCorrectly()
    {
        // Arrange
        var handlerTypes = GetHandlerTypes();
        var serviceProvider = new ServiceCollection()
            .AddApplicationHandlersFromTypes(handlerTypes)
            .BuildServiceProvider();

        // Act
        var commandHandler = serviceProvider.GetService<IRequestHandler<MockCommandRequest, MockResponse>>();
        var singleQueryHandler = serviceProvider.GetService<IRequestHandler<MockSingleQueryRequest, MockResponse>>();
        var collectionQueryHandler = serviceProvider.GetService<IRequestHandler<MockCollectionQueryRequest, MockResponse>>();

        // Assert
        commandHandler.Should().NotBeNull();
        singleQueryHandler.Should().NotBeNull();
        collectionQueryHandler.Should().NotBeNull();

        commandHandler.Should().BeOfType<MockCommandHandler>();
        singleQueryHandler.Should().BeOfType<MockSingleQueryHandler>();
        collectionQueryHandler.Should().BeOfType<MockCollectionQueryHandler>();
    }

    [Fact]
    public void ApplicationHandlerRegistrar_ShouldThrowWhenHandlerAlreadyRegistered()
    {
        // Arrange
        Type[] dupliecatedHandlerTypes = [typeof(MockCommandHandler), typeof(MockCommandHandlerDuplicated)];
        var services = new ServiceCollection();

        //Act
        Action act = () => services.AddApplicationHandlersFromTypes(dupliecatedHandlerTypes);

        // Assert
        act.Should().Throw<HandlerRegistrationAlreadyExistsException>();
    }

    public record MockResponse : IResponse
    {

    }
    public record MockCommandRequest : ICommandRequest<MockResponse>
    {
        public Guid ClientRequestId { get; } = Guid.NewGuid();
        public DateTime? ValidatedSuccessfullyAt { get; set; }
    }

    public record MockSingleQueryRequest : IQueryRequest<MockResponse>
    {
    }

    public record MockCollectionQueryRequest : IQueryRequest<MockResponse>
    {
    }

    public class MockCommandHandler : CommandHandler<MockCommandRequest, MockResponse>
    {
        protected override Task<Result<MockResponse>> HandleAsync(MockCommandRequest command, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success(new MockResponse()));
        }
    }

    public class MockCommandHandlerDuplicated : CommandHandler<MockCommandRequest, MockResponse>
    {
        protected override Task<Result<MockResponse>> HandleAsync(MockCommandRequest command, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success(new MockResponse()));
        }
    }

    public class MockSingleQueryHandler : IGetQueryResultHandler<MockSingleQueryRequest, MockResponse>
    {
        public Task<Result<MockResponse>> HandleAsync(MockSingleQueryRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Success(new MockResponse()));
        }
    }

    public class MockCollectionQueryHandler : IGetManyQueryHandler<MockCollectionQueryRequest, MockResponse>
    {
        public Task<Result<IReadOnlyList<MockResponse>>> HandleAsync(
            MockCollectionQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = Result.Success<IReadOnlyList<MockResponse>>([]);
            return Task.FromResult(result);
        }
    }

    public record MockDomainEvent : IDomainEvent
    {
    }

    public class MockEventHandler : IDomainEventHandler<MockDomainEvent>
    {
        public Task HandleAsync(MockDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

