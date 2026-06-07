using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordController(IMedicalRecordRepository medicalRecordRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _medicalRecordRepository.GetAllAsync();
            return View(records);
        }

        public async Task<IActionResult> Details(int id)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(id);
            if (record == null) return NotFound();
            return View(record);
        }
    }
}
