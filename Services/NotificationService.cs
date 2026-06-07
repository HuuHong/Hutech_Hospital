using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(int? patientId, string title, string content)
        {
            var notification = new Notification
            {
                PatientId = patientId,
                Title = title,
                Content = content,
                IsRead = false,
                CreatedAt = System.DateTime.Now
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAppointmentNotificationAsync(int patientId, string message)
        {
            await CreateNotificationAsync(patientId, "Cập nhật Lịch khám", message);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByPatientAsync(int patientId)
        {
            // Lấy thông báo cá nhân + thông báo chung (PatientId = null)
            return await _context.Notifications
                .Where(n => n.PatientId == patientId || n.PatientId == null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
