using Snebur.Application.Models.Communication;

namespace Snebur.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    public async Task<bool> SendEmailAsync(Email email)
    {
        //TODO implement email sending
        await Task.Delay(1000);
        return true;
    }
}
