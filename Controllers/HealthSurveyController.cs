using System;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Controllers
{
    [Authorize(Roles = "Patient")]
    public class HealthSurveyController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HealthSurveyController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private async Task<Patient?> GetCurrentPatientAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            return await _context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();

            // Kiem tra xem da co chua, co roi thi redirect Detail
            var existing = await _context.HealthSurveys.FirstOrDefaultAsync(h => h.PatientId == patient.Id);
            if (existing != null)
            {
                return RedirectToAction(nameof(Details));
            }

            // Init value from Patient
            var model = new HealthSurvey
            {
                Height = null,
                Weight = null,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                BloodType = ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HealthSurvey model)
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();

            var existing = await _context.HealthSurveys.FirstOrDefaultAsync(h => h.PatientId == patient.Id);
            if (existing != null)
            {
                return RedirectToAction(nameof(Details));
            }

            if (ModelState.IsValid)
            {
                model.PatientId = patient.Id;
                model.CreatedAt = DateTime.Now;
                _context.HealthSurveys.Add(model);

                // Update basic patient info if it was missing and user provided it
                if (patient.DateOfBirth == null && model.DateOfBirth != null) patient.DateOfBirth = model.DateOfBirth;
                if (string.IsNullOrEmpty(patient.Gender) && !string.IsNullOrEmpty(model.Gender)) patient.Gender = model.Gender;
                _context.Patients.Update(patient);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Lưu thông tin khảo sát sức khỏe thành công!";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();

            var record = await _context.HealthSurveys.FirstOrDefaultAsync(h => h.PatientId == patient.Id);
            if (record == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HealthSurvey model)
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();

            var record = await _context.HealthSurveys.FirstOrDefaultAsync(h => h.PatientId == patient.Id);
            if (record == null) return NotFound();

            if (ModelState.IsValid)
            {
                record.Height = model.Height;
                record.Weight = model.Weight;
                record.DateOfBirth = model.DateOfBirth;
                record.Gender = model.Gender;
                record.BloodType = model.BloodType;
                
                record.PersonalHistory = model.PersonalHistory;
                record.Allergies = model.Allergies;
                record.CurrentMedication = model.CurrentMedication;
                record.FamilyHistory = model.FamilyHistory;
                record.UnderlyingConditions = model.UnderlyingConditions;

                record.CurrentSymptoms = model.CurrentSymptoms;
                record.DiscomfortLevel = model.DiscomfortLevel;
                record.SymptomDuration = model.SymptomDuration;

                record.UpdatedAt = DateTime.Now;

                _context.HealthSurveys.Update(record);
                
                // Update basic patient info
                if (model.DateOfBirth != null) patient.DateOfBirth = model.DateOfBirth;
                if (!string.IsNullOrEmpty(model.Gender)) patient.Gender = model.Gender;
                _context.Patients.Update(patient);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin sức khỏe thành công!";
                return RedirectToAction(nameof(Details));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Forbid();

            var record = await _context.HealthSurveys.FirstOrDefaultAsync(h => h.PatientId == patient.Id);
            if (record == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(record);
        }
    }
}
