using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HUTECH_Hospital.Repositories;
using HUTECH_Hospital.Services;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly NotificationService _notificationService;

        public AppointmentController(IAppointmentRepository appointmentRepo, NotificationService notificationService)
        {
            _appointmentRepo = appointmentRepo;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(string? status, DateTime? date, string? searchString)
        {
            var appointments = await _appointmentRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(status))
            {
                appointments = appointments.Where(a => a.Status == status);
            }

            if (date.HasValue)
            {
                appointments = appointments.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                appointments = appointments.Where(a => 
                    (a.Patient != null && a.Patient.FullName != null && a.Patient.FullName.ToLower().Contains(searchString)) ||
                    (a.Doctor != null && a.Doctor.FullName != null && a.Doctor.FullName.ToLower().Contains(searchString)));
            }

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDate = date?.ToString("yyyy-MM-dd");
            ViewBag.SearchString = searchString;

            return View(appointments.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            if (appointment.Status == "Pending")
            {
                appointment.Status = "Confirmed";
                appointment.UpdatedAt = DateTime.Now;

                await _appointmentRepo.UpdateAsync(appointment);

                if (appointment.PatientId > 0)
                {
                    await _notificationService.CreateAppointmentNotificationAsync(appointment.PatientId, $"Lịch hẹn ngày {appointment.AppointmentDate:dd/MM/yyyy} đã được phê duyệt.");
                }

                TempData["Success"] = "Đã xác nhận lịch hẹn chuẩn xác.";
            }
            else
            {
                TempData["Error"] = "Chỉ có thể xác nhận lịch ở trạng thái Chờ xác nhận.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
            {
                TempData["Error"] = "Lịch đã kết thúc hoặc đã hủy, không thể hủy tiếp.";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id, string cancelReason)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
            {
                TempData["Error"] = "Lịch đã kết thúc hoặc đã bị hủy.";
                return RedirectToAction(nameof(Index));
            }

            appointment.Status = "Cancelled";
            appointment.CancelReason = cancelReason;
            appointment.UpdatedAt = DateTime.Now;

            await _appointmentRepo.UpdateAsync(appointment);

            if (appointment.PatientId > 0)
            {
                await _notificationService.CreateAppointmentNotificationAsync(appointment.PatientId, $"Lịch hẹn {appointment.AppointmentDate:dd/MM/yyyy} đã bị hủy bởi Quản trị viên.");
            }

            TempData["Success"] = "Đã hủy lịch hẹn theo yêu cầu.";
            return RedirectToAction(nameof(Index));
        }
    }
}
