using HUTECH_Hospital.Models;
using HUTECH_Hospital.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly HUTECH_Hospital.Data.ApplicationDbContext _context;

        public NotificationController(NotificationService notificationService, HUTECH_Hospital.Data.ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _context.Notifications.Include(n => n.Patient).OrderByDescending(n => n.CreatedAt).ToListAsync();
            return View(notifications);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _context.Patients
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FullName + " (" + p.StudentCode + ")" })
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification model)
        {
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError("", "Tiêu đề và nội dung không được để trống");
                ViewBag.Patients = await _context.Patients.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FullName }).ToListAsync();
                return View(model);
            }

            await _notificationService.CreateNotificationAsync(model.PatientId, model.Title, model.Content);
            TempData["SuccessMessage"] = "Gửi thông báo thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var notification = await _context.Notifications.Include(n => n.Patient).FirstOrDefaultAsync(n => n.Id == id);
            if (notification == null) return NotFound();
            return View(notification);
        }
    }
}
