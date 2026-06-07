using System;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using HUTECH_Hospital.Services;
using HUTECH_Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DoctorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly FileUploadService _fileUploadService;

        public DoctorController(
            UserManager<ApplicationUser> userManager,
            IDoctorRepository doctorRepo,
            IDepartmentRepository departmentRepo,
            FileUploadService fileUploadService)
        {
            _userManager = userManager;
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return View(doctors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentRepo.GetActiveAsync();
            var model = new DoctorCreateViewModel
            {
                Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verify user not exist
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FullName = model.FullName,
                        Gender = model.Gender,
                        DateOfBirth = model.DateOfBirth,
                        Address = model.Address,
                        CreatedAt = DateTime.Now
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Doctor");

                        var doctor = new HUTECH_Hospital.Models.Doctor
                        {
                            ApplicationUserId = user.Id,
                            DepartmentId = model.DepartmentId,
                            DoctorCode = model.DoctorCode,
                            FullName = model.FullName,
                            Gender = model.Gender,
                            DateOfBirth = model.DateOfBirth,
                            PhoneNumber = model.PhoneNumber,
                            Email = model.Email,
                            Address = model.Address,
                            Degree = model.Degree,
                            Specialization = model.Specialization,
                            ExperienceYears = model.ExperienceYears,
                            Description = model.Description,
                            IsActive = model.IsActive,
                            CreatedAt = DateTime.Now
                        };

                        if (model.AvatarFile != null)
                        {
                            doctor.AvatarUrl = await _fileUploadService.UploadFileAsync(model.AvatarFile, "doctors");
                        }

                        await _doctorRepo.AddAsync(doctor);

                        TempData["Success"] = "Tạo tài khoản và hồ sơ Bác sĩ thành công!";
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            var departments = await _departmentRepo.GetActiveAsync();
            model.Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null) return NotFound();

            var departments = await _departmentRepo.GetActiveAsync();
            var model = new DoctorEditViewModel
            {
                Id = doctor.Id,
                ApplicationUserId = doctor.ApplicationUserId,
                DepartmentId = doctor.DepartmentId,
                DoctorCode = doctor.DoctorCode ?? "",
                FullName = doctor.FullName ?? "",
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber,
                Gender = doctor.Gender,
                DateOfBirth = doctor.DateOfBirth,
                Address = doctor.Address,
                AvatarUrl = doctor.AvatarUrl,
                Degree = doctor.Degree ?? "",
                Specialization = doctor.Specialization ?? "",
                ExperienceYears = doctor.ExperienceYears,
                Description = doctor.Description,
                IsActive = doctor.IsActive,
                Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var doctor = await _doctorRepo.GetByIdAsync(id);
                if (doctor == null) return NotFound();

                // Update ApplicationUser side (Optional depending on business rule)
                var user = await _userManager.FindByIdAsync(doctor.ApplicationUserId);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Gender = model.Gender;
                    user.DateOfBirth = model.DateOfBirth;
                    user.Address = model.Address;
                    await _userManager.UpdateAsync(user);
                }

                // Update Doctor side
                doctor.DepartmentId = model.DepartmentId;
                doctor.DoctorCode = model.DoctorCode;
                doctor.FullName = model.FullName;
                doctor.PhoneNumber = model.PhoneNumber;
                doctor.Gender = model.Gender;
                doctor.DateOfBirth = model.DateOfBirth;
                doctor.Address = model.Address;
                doctor.Degree = model.Degree;
                doctor.Specialization = model.Specialization;
                doctor.ExperienceYears = model.ExperienceYears;
                doctor.Description = model.Description;
                doctor.IsActive = model.IsActive;
                doctor.UpdatedAt = DateTime.Now;

                if (model.AvatarFile != null)
                {
                    doctor.AvatarUrl = await _fileUploadService.UploadFileAsync(model.AvatarFile, "doctors");
                }

                await _doctorRepo.UpdateAsync(doctor);
                TempData["Success"] = "Cập nhật hồ sơ bác sĩ thành công!";
                return RedirectToAction(nameof(Index));
            }

            var departments = await _departmentRepo.GetActiveAsync();
            model.Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _doctorRepo.DeleteAsync(id);
            TempData["Success"] = "Đã vô hiệu hoá tài khoản Bác sĩ.";
            return RedirectToAction(nameof(Index));
        }
    }
}
