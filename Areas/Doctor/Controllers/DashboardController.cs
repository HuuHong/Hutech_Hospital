using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HUTECH_Hospital.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Lời chào hiển thị cho bác sĩ
            return View();
        }
    }
}
