using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using HUTECH_Hospital.Data;

namespace HUTECH_Hospital.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class AppointmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentRepository _appointmentRepo;

        public AppointmentController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IAppointmentRepository appointmentRepo)
        {
            _userManager = userManager;
            _context = context;
            _appointmentRepo = appointmentRepo;
        }

        private async Task<HUTECH_Hospital.Models.Doctor?> GetCurrentDoctorAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            return await _context.Doctors.FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);
        }

        public async Task<IActionResult> Index()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return Forbid();

            var appointments = await _appointmentRepo.GetByDoctorIdAsync(doctor.Id);
            return View(appointments);
        }

        public async Task<IActionResult> Today()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return Forbid();

            var appointments = await _appointmentRepo.GetByDoctorAndDateAsync(doctor.Id, DateTime.Now.Date);
            
            // Lọc ra các ca bệnh đang trong tiến trình chờ hoặc đã duyệt
            var todayAppointments = appointments
                .Where(a => a.Status == "Pending" || a.Status == "Confirmed")
                .OrderBy(a => a.AppointmentTime)
                .ToList();

            return View(todayAppointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null || appointment.DoctorId != doctor.Id) return Forbid();

            return View(appointment);
        }
    }
}
