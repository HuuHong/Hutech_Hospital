using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HUTECH_Hospital.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chuyên khoa")]
        [Display(Name = "Tên chuyên khoa")]
        public string Name { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "Tải ảnh (Tuỳ chọn)")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;
    }

    public class DoctorEditViewModel
    {
        public int Id { get; set; }
        
        public string ApplicationUserId { get; set; } = null!;

        [Required(ErrorMessage = "Bắt buộc chọn chuyên khoa")]
        [Display(Name = "Chuyên khoa")]
        public int DepartmentId { get; set; }

        public IEnumerable<SelectListItem>? Departments { get; set; }

        [Required(ErrorMessage = "Mã nhân sự (Bác sĩ) không được để rỗng")]
        [Display(Name = "Mã bác sĩ")]
        public string DoctorCode { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên Bác sĩ")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Theo Email đăng nhập")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Địa chỉ liên hệ")]
        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }

        [Display(Name = "Thay Avatar")]
        public IFormFile? AvatarFile { get; set; }

        [Required]
        [Display(Name = "Học vị")]
        public string Degree { get; set; } = null!;

        [Required]
        [Display(Name = "Chuyên môn")]
        public string Specialization { get; set; } = null!;

        [Display(Name = "Năm kinh nghiệm")]
        public int ExperienceYears { get; set; }

        [Display(Name = "Mô tả tóm tắt")]
        public string? Description { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;
    }

    public class DoctorScheduleViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Bác sĩ phụ trách")]
        public int DoctorId { get; set; }

        // Hiển thị danh sách bác sĩ trên Form Create/Edit của Admin
        public IEnumerable<SelectListItem>? Doctors { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày làm việc")]
        public DateTime WorkDate { get; set; }

        [Required]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Phòng trực")]
        public string? Room { get; set; }

        [Display(Name = "Tối đa B/N")]
        public int MaxPatients { get; set; } = 20;

        [Display(Name = "Sẵn sàng tiếp nhận")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Ghi chú thêm")]
        public string? Note { get; set; }
    }
}
