using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HUTECH_Hospital.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public Doctor? Doctor { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime WorkDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(100)]
        public string? Room { get; set; }

        public int MaxPatients { get; set; } = 20;

        public bool IsAvailable { get; set; } = true;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
