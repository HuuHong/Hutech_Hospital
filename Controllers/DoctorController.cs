using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HUTECH_Hospital.Data;
using Microsoft.AspNetCore.Mvc;

namespace HUTECH_Hospital.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? departmentId, string? keyword)
        {
            var query = _context.Doctors.Include(d => d.Department).AsQueryable();

            query = query.Where(d => d.IsActive);

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(d => d.DepartmentId == departmentId);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(d => (d.FullName != null && d.FullName.Contains(keyword)) || 
                                         (d.Specialization != null && d.Specialization.Contains(keyword)));
            }

            var doctors = await query
                .OrderByDescending(d => d.ExperienceYears)
                .ToListAsync();

            ViewBag.Departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewData["Keyword"] = keyword;
            ViewData["DepartmentId"] = departmentId;

            return View(doctors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.DoctorSchedules)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null || !doctor.IsActive) return NotFound();

            if (doctor.DoctorSchedules != null)
            {
                doctor.DoctorSchedules = doctor.DoctorSchedules
                    .Where(s => s.WorkDate >= DateTime.Now.Date && s.IsAvailable)
                    .OrderBy(s => s.WorkDate).ThenBy(s => s.StartTime).ToList();
            }

            return View(doctor);
        }
    }
}
