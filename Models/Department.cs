using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên chuyên khoa không được để trống")]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Property: 1 Department has many Doctors
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
