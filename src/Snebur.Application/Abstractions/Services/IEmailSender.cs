using Snebur.Application.Models.Communication;

namespace Snebur.Application.Abstractions.Services;

public interface IEmailSender : IApplicationService
{
    Task<bool> SendEmailAsync(Email email);
}
