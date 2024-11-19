namespace Core.Utilities.Helpers.MessageService.Abstract;

public interface ISmsService
{
    Task<bool> SendOtpSmsAsync(string phoneNumber, string otp);
}