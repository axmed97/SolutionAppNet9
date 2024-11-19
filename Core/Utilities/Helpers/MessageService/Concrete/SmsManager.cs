using Core.Utilities.Helpers.MessageService.Abstract;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Core.Utilities.Helpers.MessageService.Concrete;

public class SmsManager(HttpClient httpClient, IConfiguration configuration) : ISmsService
{

    public async Task<bool> SendOtpSmsAsync(string phoneNumber, string otp)
    {
        try
        {
            var apiUrl = configuration["SMS:Uri"];

            var data = new
            {
                Message = $"Təsdiqləmək üçün OTP kodunu daxil edin: {otp}",
                Receivers = new[] { phoneNumber },
                SendDate = DateTime.UtcNow.AddHours(4).ToString("yyyyMMdd HH:mm"),
                ExpireDate = DateTime.UtcNow.AddHours(4).AddMinutes(3).ToString("yyyyMMdd HH:mm"),
                Username = configuration["SMS:Username"],
                Password = configuration["SMS:Password"]
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(apiUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response from API: " + responseContent);

            if (response.IsSuccessStatusCode)
            {
                Log.Information("SMS sent successfully to {PhoneNumber}", phoneNumber);
                return true;
            }
            else
            {
                Log.Error("Failed to send SMS to {PhoneNumber}. StatusCode: {StatusCode}", phoneNumber, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while sending SMS to {PhoneNumber}", phoneNumber);
            return false;
        }
    }
}