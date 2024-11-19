using Core.Entities.Concrete;

namespace Core.Utilities.Helpers.MessageService.Abstract;

public interface IEmailService
{
    Task SendEmailAsync(EmailMetadata emailData);
}