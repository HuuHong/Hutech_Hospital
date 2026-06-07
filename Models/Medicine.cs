using System;
using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class Medicine
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thuốc")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính")]
        public string Unit { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Usage { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
    }
}
