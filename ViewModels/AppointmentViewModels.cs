using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HUTECH_Hospital.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int DoctorId { get; set; }

        public int? DoctorScheduleId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày khám")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; } = DateTime.Now.Date.AddDays(1);

        [Required(ErrorMessage = "Vui lòng chọn giờ khám")]
        public TimeSpan AppointmentTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do khám")]
        [StringLength(500, ErrorMessage = "Lý do khám không vượt quá 500 ký tự")]
        public string Reason { get; set; } = string.Empty;

        public IEnumerable<SelectListItem>? Departments { get; set; }
        public IEnumerable<SelectListItem>? Doctors { get; set; }
        public IEnumerable<SelectListItem>? Schedules { get; set; }
    }

    public class AppointmentEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int DoctorId { get; set; }

        public int? DoctorScheduleId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do khám")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public IEnumerable<SelectListItem>? Departments { get; set; }
        public IEnumerable<SelectListItem>? Doctors { get; set; }
        public IEnumerable<SelectListItem>? Schedules { get; set; }
    }

    public class AppointmentDetailViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? CancelReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentStatusViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? CancelReason { get; set; }
    }
}
