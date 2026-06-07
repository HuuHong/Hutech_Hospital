using HUTECH_Hospital.Data;
using HUTECH_Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var model = new DashboardStatisticViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(d => d.IsActive),
                TotalDepartments = await _context.Departments.CountAsync(d => d.IsActive),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TodayAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == today),
                PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending"),
                CompletedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Completed"),
                TotalMedicalRecords = await _context.MedicalRecords.CountAsync(),
                TotalFeedbacks = await _context.Feedbacks.CountAsync(),
                UnresolvedFeedbacks = await _context.Feedbacks.CountAsync(f => !f.IsResolved)
            };

            return View(model);
        }
    }
}
