using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Demo data cho Dashboard
            ViewBag.TotalUsers = 150;
            ViewBag.TotalPatients = 120;
            ViewBag.TotalDoctors = 28;
            
            return View();
        }
    }
}
