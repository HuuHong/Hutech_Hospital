using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HUTECH_Hospital.Data;
using Microsoft.AspNetCore.Mvc;

namespace HUTECH_Hospital.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(d => d.Name.Contains(keyword));
            }

            var departments = await query
                .Include(d => d.Doctors)
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            ViewData["Keyword"] = keyword;
            return View(departments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync(d => d.Id == id);
                
            if (department == null || !department.IsActive) return NotFound();
            
            return View(department);
        }
    }
}
