using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HUTECH_Hospital.Services
{
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task CreateAppointmentNotificationAsync(int patientId, string message)
        {
            // Trong tương lai có thể lưu vào DB Notifications
            // Tạm thời log ra Console
            _logger.LogInformation($"[NOTIFICATION] To Patient {patientId}: {message}");
            return Task.CompletedTask;
        }
    }
}
