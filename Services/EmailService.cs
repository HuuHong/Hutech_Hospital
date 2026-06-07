using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HUTECH_Hospital.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAppointmentConfirmationAsync(string email, string subject, string message)
        {
            // Tích hợp SMTP gửi email thực tế tại đây
            _logger.LogInformation($"[EMAIL SENT] To: {email} | Subject: {subject} | Content: {message}");
            return Task.CompletedTask;
        }
    }
}
