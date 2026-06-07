using System;
using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
