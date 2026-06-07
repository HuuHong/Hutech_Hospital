using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HUTECH_Hospital.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public DoctorsController(IDoctorRepository doctorRepo, IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<IActionResult> Index(int? departmentId, string? keyword)
        {
            var doctors = string.IsNullOrWhiteSpace(keyword) 
                ? await _doctorRepo.GetActiveAsync() 
                : await _doctorRepo.SearchAsync(keyword);

            if (departmentId.HasValue && departmentId > 0)
            {
                doctors = doctors.Where(d => d.DepartmentId == departmentId).ToList();
            }

            var departments = await _departmentRepo.GetActiveAsync();
            ViewBag.Departments = departments;
            
            ViewData["Keyword"] = keyword;
            ViewData["DepartmentId"] = departmentId;
            
            return View(doctors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null || !doctor.IsActive) return NotFound();

            // Order schedule (closest valid coming schedule to display)
            if (doctor.DoctorSchedules != null)
            {
                doctor.DoctorSchedules = doctor.DoctorSchedules
                    .Where(s => s.WorkDate >= System.DateTime.Now.Date && s.IsAvailable)
                    .OrderBy(s => s.WorkDate).ThenBy(s => s.StartTime).ToList();
            }

            return View(doctor);
        }
    }
}
