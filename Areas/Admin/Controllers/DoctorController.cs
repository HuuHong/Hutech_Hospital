using System;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DoctorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DoctorController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors.Include(d => d.ApplicationUser).ToListAsync();
            return View(doctors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new DoctorCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorCreateViewModel model)
        {
            if (ModelState.IsValid)
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
                    // Tự động gán quyền Doctor
                    await _userManager.AddToRoleAsync(user, "Doctor");

                    var doctor = new HUTECH_Hospital.Models.Doctor
                    {
                        ApplicationUserId = user.Id,
                        DoctorCode = model.DoctorCode,
                        FullName = model.FullName,
                        Gender = model.Gender,
                        DateOfBirth = model.DateOfBirth,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        Address = model.Address,
                        Degree = model.Degree,
                        Specialization = model.Specialization,
                        CreatedAt = DateTime.Now
                    };

                    _context.Doctors.Add(doctor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo tài khoản Bác sĩ thành công!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
