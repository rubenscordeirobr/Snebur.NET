using Snebur.Application.Abstractions.Handlers;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.Application.UnitTests.Events;

public class DomainEventContextTests
{
    [Fact]
    public void Constructor_ShouldInitializeEvents()
    {
        // Arrange
        var events = new List<IDomainEvent> { new MockDomainEvent() };

        // Act
        var session = AnonymousUserConstants.AnonymousUserSession;
        var context = new DomainEventContext(session, events);

        // Assert
        context.Events.Should().BeEquivalentTo(events);
    }

    [Fact]
    public void Cancel_ShouldSetIsCanceledAndError()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var context = new DomainEventContext(session, new List<IDomainEvent>());
        var error = new DomainEventError("DomainTest.Error", "Test error");

        // Act
        context.Cancel(error);

        // Assert
        context.IsCanceled.Should().BeTrue();
        context.Error.Should().Be(error);
    }

    [Fact]
    public void Cancel_WhenAlreadyLocked_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var context = new DomainEventContext(session, new List<IDomainEvent>());
        context.LockCancellation();
        var error = new DomainEventError("DomainTest.Error", "Test error");

        // Act
        Action act = () => context.Cancel(error);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("The context cannot be canceled.");
    }

    [Fact]
    public void GetException_WhenCanceled_ShouldReturnExceptionWithErrorMessage()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var context = new DomainEventContext(session, []);
        var error = new DomainEventError("DomainTest.Error", "Test error");
        context.Cancel(error);

        // Act
        var exception = context.Exception;

        context.IsCanceled
            .Should()
            .BeTrue();

        // Assert
        exception!.Message.Should().Be("Test error");
    }

    [Fact]
    public void AddExecutedEventResults_ShouldAddResults()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var context = new DomainEventContext(session, new List<IDomainEvent>());
        var domainEvent = new MockDomainEvent();
        var results = new List<ExecutedDomainEventResult>
        {
            new ExecutedDomainEventResult(domainEvent, typeof(MockHandler), null, null, null)
        };

        // Act
        context.AddExecutedEventResults(domainEvent, results);

        // Assert
        context.GetExecutedEventResults(domainEvent)
            .Should()
            .BeEquivalentTo(results);
    }

    [Fact]
    public void GetExecutedEventResults_WhenNoResults_ShouldReturnEmptyList()
    {
        // Arrange
        var session = AnonymousUserConstants.AnonymousUserSession;
        var context = new DomainEventContext(session, new List<IDomainEvent>());
        var domainEvent = new MockDomainEvent();

        // Act
        var results = context.GetExecutedEventResults(domainEvent);

        // Assert
        results.Should().BeEmpty();
    }
    public class MockDomainEvent : IDomainEvent { }

    public class MockHandler : IApplicationHandler
    {
        public Task HandleAsync(object handlerObject, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}

