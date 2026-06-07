using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Controllers
{
    [Authorize(Roles = "Patient")]
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HUTECH_Hospital.Data.ApplicationDbContext _context;

        public MedicalRecordController(IMedicalRecordRepository medicalRecordRepository, UserManager<ApplicationUser> userManager, HUTECH_Hospital.Data.ApplicationDbContext context)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var patient = _context.Patients.FirstOrDefault(p => p.ApplicationUserId == user.Id);
            if (patient == null) return NotFound("Hồ sơ bệnh nhân không tồn tại.");

            var records = await _medicalRecordRepository.GetByPatientIdAsync(patient.Id);
            return View(records);
        }

        public async Task<IActionResult> Details(int id)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(id);
            if (record == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (record.Patient?.ApplicationUserId != user?.Id) return Forbid();

            return View(record);
        }
    }
}
