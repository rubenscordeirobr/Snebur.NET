using Snebur.Application.Models.Communication;

namespace Snebur.Testing.Core.Mocks.Infrastructure;

public sealed class EmailSenderMock : IEmailSender
{
    public Task<bool> SendEmailAsync(Email email)
    {
        return Task.FromResult(true);
    }
}
