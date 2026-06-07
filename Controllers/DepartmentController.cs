using System.Threading.Tasks;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HUTECH_Hospital.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepo;

        public DepartmentController(IDepartmentRepository departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            var departments = await _departmentRepo.SearchAsync(keyword ?? string.Empty);
            ViewData["Keyword"] = keyword;
            return View(departments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null || !department.IsActive) return NotFound();
            
            return View(department);
        }
    }
}
