using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using HUTECH_Hospital.Services;
using HUTECH_Hospital.ViewModels;
using HUTECH_Hospital.Data;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Controllers
{
    [Authorize(Roles = "Patient")]
    public class AppointmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly AppointmentService _appointmentService;
        private readonly NotificationService _notificationService;
        private readonly EmailService _emailService;

        public AppointmentController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IAppointmentRepository appointmentRepo,
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo,
            AppointmentService appointmentService,
            NotificationService notificationService,
            EmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _appointmentRepo = appointmentRepo;
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
            _appointmentService = appointmentService;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        private async Task<Patient?> GetCurrentPatientAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            return await _context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? doctorId)
        {
            var departments = await _departmentRepo.GetActiveAsync();
            var doctors = await _doctorRepo.GetActiveAsync();

            var model = new AppointmentCreateViewModel
            {
                Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }),
                Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" })
            };

            if (doctorId.HasValue)
            {
                var doc = await _doctorRepo.GetByIdAsync(doctorId.Value);
                if (doc != null && doc.IsActive)
                {
                    model.DoctorId = doc.Id;
                    model.DepartmentId = doc.DepartmentId;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();

            if (ModelState.IsValid)
            {
                if (await _appointmentService.CanBookAppointmentAsync(model.DoctorId, model.AppointmentDate, model.AppointmentTime))
                {
                    var appointment = new Appointment
                    {
                        PatientId = patient.Id,
                        DoctorId = model.DoctorId,
                        DepartmentId = model.DepartmentId,
                        DoctorScheduleId = model.DoctorScheduleId,
                        AppointmentDate = model.AppointmentDate,
                        AppointmentTime = model.AppointmentTime,
                        Reason = model.Reason,
                        Status = "Pending",
                        CreatedAt = DateTime.Now
                    };

                    await _appointmentRepo.AddAsync(appointment);

                    await _notificationService.CreateAppointmentNotificationAsync(patient.Id, "Bạn đã gửi yêu cầu đặt lịch khám thành công.");
                    if (!string.IsNullOrEmpty(patient.ApplicationUser?.Email))
                    {
                        await _emailService.SendAppointmentConfirmationAsync(patient.ApplicationUser.Email, "Xác nhận đặt lịch", "Lịch của bạn đang chờ xác nhận.");
                    }

                    TempData["Success"] = "Đặt lịch khám thành công! Vui lòng chờ xác nhận từ bệnh viện.";
                    return RedirectToAction(nameof(MyAppointments));
                }
                else
                {
                    ModelState.AddModelError("", "Thời gian này không hợp lệ hoặc bác sĩ đã có lịch hẹn/không có ca trực trong khung giờ này.");
                }
            }

            var departments = await _departmentRepo.GetActiveAsync();
            var doctors = await _doctorRepo.GetActiveAsync();
            model.Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
            model.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" });
            return View(model);
        }

        public async Task<IActionResult> MyAppointments()
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();

            var appointments = await _appointmentRepo.GetByPatientIdAsync(patient.Id);
            return View(appointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            /*var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();*/

            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            var patient = await GetCurrentPatientAsync();
            if (patient == null || appointment.PatientId != patient.Id) return Forbid();

            var model = new AppointmentDetailViewModel
            {
                Id = appointment.Id,
                PatientName = appointment.Patient?.FullName ?? "",
                PatientPhone = appointment.Patient?.PhoneNumber ?? "",
                DoctorName = appointment.Doctor?.FullName ?? "",
                DepartmentName = appointment.Department?.Name ?? "",
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Reason = appointment.Reason,
                Status = appointment.Status,
                Note = appointment.Note,
                CancelReason = appointment.CancelReason,
                CreatedAt = appointment.CreatedAt
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            var patient = await GetCurrentPatientAsync();
            if (patient == null || appointment.PatientId != patient.Id) return Forbid();

            if (!_appointmentService.CanEditOrCancelAsync(appointment))
            {
                TempData["Error"] = "Không thể sửa lịch hẹn đã hoàn tất hoặc đã hủy.";
                return RedirectToAction(nameof(MyAppointments));
            }

            var departments = await _departmentRepo.GetActiveAsync();
            var doctors = await _doctorRepo.GetActiveAsync();

            var model = new AppointmentEditViewModel
            {
                Id = appointment.Id,
                DepartmentId = appointment.DepartmentId,
                DoctorId = appointment.DoctorId,
                DoctorScheduleId = appointment.DoctorScheduleId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Reason = appointment.Reason,
                Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }),
                Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            var patient = await GetCurrentPatientAsync();
            if (patient == null || appointment.PatientId != patient.Id) return Forbid();

            if (!_appointmentService.CanEditOrCancelAsync(appointment))
            {
                TempData["Error"] = "Không thể sửa lịch hẹn đã hoàn tất hoặc đã hủy.";
                return RedirectToAction(nameof(MyAppointments));
            }

            if (ModelState.IsValid)
            {
                if (await _appointmentService.CanBookAppointmentAsync(model.DoctorId, model.AppointmentDate, model.AppointmentTime, id))
                {
                    appointment.DepartmentId = model.DepartmentId;
                    appointment.DoctorId = model.DoctorId;
                    appointment.DoctorScheduleId = model.DoctorScheduleId;
                    appointment.AppointmentDate = model.AppointmentDate;
                    appointment.AppointmentTime = model.AppointmentTime;
                    appointment.Reason = model.Reason;
                    appointment.UpdatedAt = DateTime.Now;

                    await _appointmentRepo.UpdateAsync(appointment);
                    TempData["Success"] = "Cập nhật lịch hẹn thành công.";
                    return RedirectToAction(nameof(MyAppointments));
                }
                else
                {
                    ModelState.AddModelError("", "Thời gian cập nhật không hợp lệ hoặc bị trùng lịch.");
                }
            }

            var departments = await _departmentRepo.GetActiveAsync();
            var doctors = await _doctorRepo.GetActiveAsync();
            model.Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
            model.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            var patient = await GetCurrentPatientAsync();
            if (patient == null || appointment.PatientId != patient.Id) return Forbid();

            if (!_appointmentService.CanEditOrCancelAsync(appointment))
            {
                TempData["Error"] = "Không thể hủy lịch hẹn đã hoàn tất hoặc đã chuyển trạng thái Hủy.";
                return RedirectToAction(nameof(MyAppointments));
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id, string cancelReason)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            var patient = await GetCurrentPatientAsync();
            if (patient == null || appointment.PatientId != patient.Id) return Forbid();

            if (!_appointmentService.CanEditOrCancelAsync(appointment))
            {
                TempData["Error"] = "Không thể hủy lịch này.";
                return RedirectToAction(nameof(MyAppointments));
            }

            appointment.Status = "Cancelled";
            appointment.CancelReason = cancelReason;
            appointment.UpdatedAt = DateTime.Now;

            await _appointmentRepo.UpdateAsync(appointment);
            await _notificationService.CreateAppointmentNotificationAsync(patient.Id, "Bạn đã hủy lịch khám thành công.");

            TempData["Success"] = "Đã hủy lịch hẹn.";
            return RedirectToAction(nameof(MyAppointments));
        }

        // Endpoint for fetching doctors via AJAX
        [HttpGet]
        public async Task<IActionResult> GetDoctorsByDepartment(int departmentId)
        {
            var doctors = await _doctorRepo.GetActiveAsync();
            var filtered = doctors.Where(d => d.DepartmentId == departmentId)
                                  .Select(d => new { id = d.Id, name = $"{d.DoctorCode} - {d.FullName}" });
            return Json(filtered);
        }
    }
}
