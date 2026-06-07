using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Controllers
{
    [Authorize(Roles = "Patient")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HUTECH_Hospital.Data.ApplicationDbContext _context;

        public FeedbackController(IFeedbackRepository feedbackRepository, UserManager<ApplicationUser> userManager, HUTECH_Hospital.Data.ApplicationDbContext context)
        {
            _feedbackRepository = feedbackRepository;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var patient = _context.Patients.FirstOrDefault(p => p.ApplicationUserId == user.Id);
                if (patient == null) return NotFound("Hồ sơ bệnh nhân không tồn tại.");

                model.PatientId = patient.Id;
                model.IsResolved = false;
                model.CreatedAt = System.DateTime.Now;
                
                await _feedbackRepository.AddAsync(model);
                TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi phản hồi!";
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
