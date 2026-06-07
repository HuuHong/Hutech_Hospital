using System;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using HUTECH_Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DoctorScheduleController : Controller
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IDoctorRepository _doctorRepo;

        public DoctorScheduleController(IDoctorScheduleRepository scheduleRepo, IDoctorRepository doctorRepo)
        {
            _scheduleRepo = scheduleRepo;
            _doctorRepo = doctorRepo;
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _scheduleRepo.GetAllAsync();
            return View(schedules);
        }

        public async Task<IActionResult> Details(int id)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        public async Task<IActionResult> Create()
        {
            var doctors = await _doctorRepo.GetActiveAsync();
            var model = new DoctorScheduleViewModel
            {
                Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" }),
                WorkDate = DateTime.Now.Date.AddDays(1)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.EndTime <= model.StartTime)
                {
                    ModelState.AddModelError("EndTime", "Giờ kết thúc phải lớn hơn giờ bắt đầu!");
                }
                else
                {
                    // Check overlapping schedules
                    var devSchedules = await _scheduleRepo.GetByDoctorIdAsync(model.DoctorId);
                    var overlapping = devSchedules.Any(s => s.WorkDate.Date == model.WorkDate.Date &&
                                                            ((model.StartTime >= s.StartTime && model.StartTime < s.EndTime) ||
                                                             (model.EndTime > s.StartTime && model.EndTime <= s.EndTime) ||
                                                             (model.StartTime <= s.StartTime && model.EndTime >= s.EndTime)));
                    if (overlapping)
                    {
                        ModelState.AddModelError("", "Khung giờ này đã bị trùng lắp với một ca làm việc khác của bác sĩ trong cùng ngày.");
                    }
                    else
                    {
                        var schedule = new DoctorSchedule
                        {
                            DoctorId = model.DoctorId,
                            WorkDate = model.WorkDate,
                            StartTime = model.StartTime,
                            EndTime = model.EndTime,
                            Room = model.Room,
                            MaxPatients = model.MaxPatients,
                            IsAvailable = model.IsAvailable,
                            Note = model.Note,
                            CreatedAt = DateTime.Now
                        };
                        
                        await _scheduleRepo.AddAsync(schedule);
                        TempData["Success"] = "Đã xếp lịch trực mới thành công cho bác sĩ.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            var doctors = await _doctorRepo.GetActiveAsync();
            model.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" });
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(id);
            if (schedule == null) return NotFound();

            var doctors = await _doctorRepo.GetActiveAsync();
            var model = new DoctorScheduleViewModel
            {
                Id = schedule.Id,
                DoctorId = schedule.DoctorId,
                WorkDate = schedule.WorkDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                MaxPatients = schedule.MaxPatients,
                IsAvailable = schedule.IsAvailable,
                Note = schedule.Note,
                Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorScheduleViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (model.EndTime <= model.StartTime)
                {
                    ModelState.AddModelError("EndTime", "Giờ kết thúc phải lớn hơn giờ bắt đầu!");
                }
                else
                {
                    // Check overlapping schedules (excluding self)
                    var devSchedules = await _scheduleRepo.GetByDoctorIdAsync(model.DoctorId);
                    var overlapping = devSchedules.Any(s => s.Id != id && s.WorkDate.Date == model.WorkDate.Date &&
                                                            ((model.StartTime >= s.StartTime && model.StartTime < s.EndTime) ||
                                                             (model.EndTime > s.StartTime && model.EndTime <= s.EndTime) ||
                                                             (model.StartTime <= s.StartTime && model.EndTime >= s.EndTime)));
                    if (overlapping)
                    {
                        ModelState.AddModelError("", "Khung giờ cập nhật bị trùng với ca làm việc khác trong cùng ngày.");
                    }
                    else
                    {
                        var schedule = await _scheduleRepo.GetByIdAsync(id);
                        if (schedule == null) return NotFound();

                        schedule.DoctorId = model.DoctorId;
                        schedule.WorkDate = model.WorkDate;
                        schedule.StartTime = model.StartTime;
                        schedule.EndTime = model.EndTime;
                        schedule.Room = model.Room;
                        schedule.MaxPatients = model.MaxPatients;
                        schedule.IsAvailable = model.IsAvailable;
                        schedule.Note = model.Note;
                        schedule.UpdatedAt = DateTime.Now;

                        await _scheduleRepo.UpdateAsync(schedule);
                        TempData["Success"] = "Lưu sửa ca trực hoàn tất.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            var doctors = await _doctorRepo.GetActiveAsync();
            model.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = $"{d.DoctorCode} - {d.FullName}" });
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _scheduleRepo.DeleteAsync(id);
            TempData["Success"] = "Ca trực biểu đã được bãi bỏ.";
            return RedirectToAction(nameof(Index));
        }
    }
}
