using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HUTECH_Hospital.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class ScheduleController : Controller
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HUTECH_Hospital.Data.ApplicationDbContext _context;

        public ScheduleController(IDoctorScheduleRepository scheduleRepo, UserManager<ApplicationUser> userManager, HUTECH_Hospital.Data.ApplicationDbContext context)
        {
            _scheduleRepo = scheduleRepo;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy user đăng nhập hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Tìm thông tin Doctor theo ApplicationUserId (Bác sĩ chỉ truy cập tài khoản ứng với ID này)
            var currentDoctor = _context.Doctors.FirstOrDefault(d => d.ApplicationUserId == user.Id);
            
            if (currentDoctor == null)
            {
                // Account là doctor nhưng chưa tạo record doctor profile
                return Content("Tài khoản chưa được gán Hồ sơ Bác Sĩ. Lỗi hệ thống.");
            }

            // Truy xuất ca khám riêng của đúng bác sĩ này
            var selfSchedules = await _scheduleRepo.GetByDoctorIdAsync(currentDoctor.Id);

            return View(selfSchedules);
        }
    }
}
