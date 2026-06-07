using System;

namespace HUTECH_Hospital.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
