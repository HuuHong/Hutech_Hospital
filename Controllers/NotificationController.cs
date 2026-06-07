using HUTECH_Hospital.Models;
using HUTECH_Hospital.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Controllers
{
    [Authorize(Roles = "Patient")]
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HUTECH_Hospital.Data.ApplicationDbContext _context;

        public NotificationController(NotificationService notificationService, UserManager<ApplicationUser> userManager, HUTECH_Hospital.Data.ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var patient = _context.Patients.FirstOrDefault(p => p.ApplicationUserId == user.Id);
            if (patient == null) return NotFound("Hồ sơ bệnh nhân không tồn tại.");

            var notifications = await _notificationService.GetNotificationsByPatientAsync(patient.Id);
            return View(notifications);
        }

        public async Task<IActionResult> Details(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            // Giả lập view
            return RedirectToAction("Index");
        }
    }
}
