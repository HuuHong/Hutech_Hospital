using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PatientProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

            if (patient == null) return NotFound("Không tìm thấy hồ sơ người bệnh.");

            var model = new PatientProfileViewModel
            {
                FullName = patient.FullName ?? user.FullName ?? "",
                StudentCode = patient.StudentCode,
                Gender = patient.Gender ?? user.Gender,
                DateOfBirth = patient.DateOfBirth ?? user.DateOfBirth,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address ?? user.Address,
                AvatarUrl = patient.AvatarUrl ?? user.AvatarUrl
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
            if (patient == null) return NotFound();

            var model = new PatientProfileViewModel
            {
                FullName = patient.FullName ?? "",
                StudentCode = patient.StudentCode,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address,
                AvatarUrl = patient.AvatarUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
            if (patient == null) return NotFound();

            // Lấy dữ liệu update
            patient.FullName = model.FullName;
            patient.StudentCode = model.StudentCode;
            patient.Gender = model.Gender;
            patient.DateOfBirth = model.DateOfBirth;
            patient.PhoneNumber = model.PhoneNumber;
            patient.Address = model.Address;
            patient.AvatarUrl = model.AvatarUrl;

            // Đồng bộ dữ liệu sang bảng ApplicationUser nếu cần thiết (tùy nghiệp vụ)
            user.FullName = model.FullName;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.Address = model.Address;
            user.AvatarUrl = model.AvatarUrl;

            _context.Patients.Update(patient);
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
