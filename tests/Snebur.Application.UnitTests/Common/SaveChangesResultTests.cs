using Snebur.Application.Common;
using Snebur.Application.Abstractions.Events;
using Moq;

namespace Snebur.Application.UnitTests.Common;

public class SaveChangesResultTests
{
    private readonly IDomainEventContext _domainEventContext = new Mock<IDomainEventContext>().Object;

    [Fact]
    public void Constructor_WithError_ShouldSetPropertiesCorrectly()
    {
        var error = new DomainEventError("code", "message");

        var result = new SaveChangesResult(_domainEventContext, error);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.AffectedRows.Should().Be(0);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().BeOfType<DomainEventException>();
    }

    [Fact]
    public void Constructor_WithAffectedRows_ShouldSetPropertiesCorrectly()
    {
        var affectedRows = 5;

        var result = new SaveChangesResult(_domainEventContext, affectedRows);

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
        result.AffectedRows.Should().Be(affectedRows);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void Constructor_Success_ShouldReturnSuccessResult()
    {
        var affectedRows = 5;

        var result = SaveChangesResult.Success(_domainEventContext, affectedRows);

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
        result.AffectedRows.Should().Be(affectedRows);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void SaveChangesError_ShouldReturnErrorResult()
    {
        var exception = new Exception("error");

        var result = SaveChangesResult.SaveChangesError(exception, _domainEventContext);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.AffectedRows.Should().Be(0);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void TransactionRollbackError_ShouldReturnErrorResult()
    {
        var exception = new Exception("error");

        var result = SaveChangesResult.TransactionRollbackError(exception, _domainEventContext);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.AffectedRows.Should().Be(0);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void DomainEventError_ShouldReturnErrorResult()
    {
        var error = new DomainEventError("code", "message");

        var result = SaveChangesResult.DomainEventError(_domainEventContext, error);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.AffectedRows.Should().Be(0);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().BeOfType<DomainEventException>();
    }

    [Fact]
    public void OperationCanceledError_WithCancellationToken_ShouldReturnErrorResult()
    {
        var cancellationToken = new CancellationToken(true);

        var result = SaveChangesResult.OperationCanceledError(_domainEventContext, cancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.AffectedRows.Should().Be(0);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().BeOfType<OperationCanceledException>();
    }

    [Fact]
    public void OperationCanceledError_WithException_ShouldReturnErrorResult()
    {
        var exception = new OperationCanceledException("error");

        var result = SaveChangesResult.OperationCanceledError(exception, _domainEventContext);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.AffectedRows.Should().Be(0);
        result.DomainEventContext.Should().Be(_domainEventContext);
        result.Exception.Should().Be(exception);
    }
}
