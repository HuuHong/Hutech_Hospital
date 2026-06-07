using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackController(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IActionResult> Index()
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();
            return View(feedbacks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null) return NotFound();
            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string reply)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null) return NotFound();

            feedback.Reply = reply;
            feedback.IsResolved = true;
            feedback.UpdatedAt = System.DateTime.Now;
            
            await _feedbackRepository.UpdateAsync(feedback);
            TempData["SuccessMessage"] = "Đã gửi phản hồi thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkResolved(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsResolved = true;
            feedback.UpdatedAt = System.DateTime.Now;
            
            await _feedbackRepository.UpdateAsync(feedback);
            TempData["SuccessMessage"] = "Đã đánh dấu xử lý xong!";
            return RedirectToAction(nameof(Index));
        }
    }
}
