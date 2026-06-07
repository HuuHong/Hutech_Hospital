using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using HUTECH_Hospital.Services;
using HUTECH_Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HUTECH_Hospital.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationService _notificationService;

        public MedicalRecordController(
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            IMedicineRepository medicineRepository,
            UserManager<ApplicationUser> userManager,
            NotificationService notificationService)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _medicineRepository = medicineRepository;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GetCurrentDoctorAsync removed

        // GET: /Doctor/MedicalRecord/Create?appointmentId=1
        public async Task<IActionResult> Create(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Lịch hẹn không tồn tại.";
                return RedirectToAction("Today", "Appointment");
            }

            var doctorUser = await _userManager.GetUserAsync(User);
            // Check if this appointment belongs to the logged in doctor
            if (appointment.Doctor?.ApplicationUserId != doctorUser?.Id)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thao tác trên ca khám này.";
                return RedirectToAction("Today", "Appointment");
            }

            if (appointment.Status != "Confirmed")
            {
                TempData["ErrorMessage"] = "Chỉ có thể tạo kết quả cho lịch khám đã được xác nhận (Confirmed).";
                return RedirectToAction("Details", "Appointment", new { id = appointmentId });
            }

            var existingRecord = await _medicalRecordRepository.GetByAppointmentIdAsync(appointmentId);
            if (existingRecord != null)
            {
                TempData["InfoMessage"] = "Kết quả khám đã được tạo trước đó.";
                return RedirectToAction("Edit", new { id = existingRecord.Id });
            }

            var medicines = await _medicineRepository.GetActiveAsync();
            var medicineSelectList = medicines.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Name} ({m.Unit})"
            }).ToList();

            var viewModel = new MedicalRecordViewModel
            {
                AppointmentId = appointmentId,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient?.FullName ?? "",
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor?.FullName ?? "",
                // Init one empty prescription detail row
                PrescriptionDetails = new List<PrescriptionDetailViewModel>
                {
                    new PrescriptionDetailViewModel { Medicines = medicineSelectList }
                }
            };

            ViewBag.Medicines = medicineSelectList;
            return View(viewModel);
        }

        // POST: /Doctor/MedicalRecord/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordViewModel model)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(model.AppointmentId);
            if (appointment == null) return NotFound();

            if (!ModelState.IsValid)
            {
                var medicines = await _medicineRepository.GetActiveAsync();
                ViewBag.Medicines = medicines.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.Name} ({m.Unit})"
                }).ToList();
                return View(model);
            }

            // Create Record
            var record = new MedicalRecord
            {
                AppointmentId = model.AppointmentId,
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                Diagnosis = model.Diagnosis,
                Symptoms = model.Symptoms,
                Treatment = model.Treatment,
                DoctorNote = model.DoctorNote,
                ReExaminationDate = model.ReExaminationDate,
                CreatedAt = DateTime.Now
            };

            // Check if there are valid prescription items
            var validPrescriptions = model.PrescriptionDetails?.Where(p => p.MedicineId > 0 && p.Quantity > 0).ToList();
            if (validPrescriptions != null && validPrescriptions.Any())
            {
                record.Prescription = new Prescription
                {
                    Note = "Đơn thuốc tự động kê",
                    CreatedAt = DateTime.Now,
                    PrescriptionDetails = validPrescriptions.Select(p => new PrescriptionDetail
                    {
                        MedicineId = p.MedicineId,
                        Quantity = p.Quantity,
                        Dosage = p.Dosage,
                        Usage = p.Usage,
                        Note = p.Note
                    }).ToList()
                };
            }

            await _medicalRecordRepository.AddAsync(record);

            // Update Appointment Status
            appointment.Status = "Completed";
            appointment.UpdatedAt = DateTime.Now;
            await _appointmentRepository.UpdateAsync(appointment);

            // Notify patient
            await _notificationService.CreateNotificationAsync(model.PatientId, 
                "Kết quả khám đã được cập nhật", 
                $"Bác sĩ {model.DoctorName} đã cập nhật kết quả khám và đơn thuốc cho ca khám ngày {appointment.AppointmentDate:dd/MM/yyyy}.");

            TempData["SuccessMessage"] = "Đã lưu kết quả khám thành công!";
            return RedirectToAction("Details", new { id = record.Id });
        }

        // GET: /Doctor/MedicalRecord/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(id);
            if (record == null) return NotFound();

            var doctorUser = await _userManager.GetUserAsync(User);
            if (record.Doctor?.ApplicationUserId != doctorUser?.Id)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền sửa hồ sơ này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var medicines = await _medicineRepository.GetActiveAsync();
            var medicineSelectList = medicines.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Name} ({m.Unit})"
            }).ToList();

            var model = new MedicalRecordViewModel
            {
                Id = record.Id,
                AppointmentId = record.AppointmentId,
                PatientId = record.PatientId,
                PatientName = record.Patient?.FullName ?? "",
                DoctorId = record.DoctorId,
                DoctorName = record.Doctor?.FullName ?? "",
                Diagnosis = record.Diagnosis,
                Symptoms = record.Symptoms,
                Treatment = record.Treatment,
                DoctorNote = record.DoctorNote,
                ReExaminationDate = record.ReExaminationDate,
                PrescriptionDetails = record.Prescription?.PrescriptionDetails.Select(pd => new PrescriptionDetailViewModel
                {
                    MedicineId = pd.MedicineId,
                    MedicineName = pd.Medicine?.Name ?? "",
                    Quantity = pd.Quantity,
                    Dosage = pd.Dosage,
                    Usage = pd.Usage,
                    Note = pd.Note,
                    Medicines = medicineSelectList
                }).ToList() ?? new List<PrescriptionDetailViewModel>()
            };

            if (!model.PrescriptionDetails.Any())
            {
                model.PrescriptionDetails.Add(new PrescriptionDetailViewModel { Medicines = medicineSelectList });
            }

            ViewBag.Medicines = medicineSelectList;
            return View(model);
        }

        // POST: /Doctor/MedicalRecord/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecordViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var medicines = await _medicineRepository.GetActiveAsync();
                ViewBag.Medicines = medicines.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.Name} ({m.Unit})"
                }).ToList();
                return View(model);
            }

            var record = await _medicalRecordRepository.GetByIdAsync(id);
            if (record == null) return NotFound();

            record.Diagnosis = model.Diagnosis;
            record.Symptoms = model.Symptoms;
            record.Treatment = model.Treatment;
            record.DoctorNote = model.DoctorNote;
            record.ReExaminationDate = model.ReExaminationDate;
            record.UpdatedAt = DateTime.Now;

            // Basic prescription update: clear and recreate (in reality, should be smarter)
            if (record.Prescription != null)
            {
                record.Prescription.PrescriptionDetails.Clear(); // Require cascade in EF or manual removal
            }
            else
            {
                record.Prescription = new Prescription { Note = "Cập nhật đơn thuốc", CreatedAt = DateTime.Now };
            }

            var validPrescriptions = model.PrescriptionDetails?.Where(p => p.MedicineId > 0 && p.Quantity > 0).ToList();
            if (validPrescriptions != null && validPrescriptions.Any())
            {
                foreach(var p in validPrescriptions)
                {
                    record.Prescription.PrescriptionDetails.Add(new PrescriptionDetail
                    {
                        MedicineId = p.MedicineId,
                        Quantity = p.Quantity,
                        Dosage = p.Dosage,
                        Usage = p.Usage,
                        Note = p.Note
                    });
                }
            }

            await _medicalRecordRepository.UpdateAsync(record);
            TempData["SuccessMessage"] = "Cập nhật kết quả khám thành công!";
            return RedirectToAction("Details", new { id = record.Id });
        }

        // GET: /Doctor/MedicalRecord/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(id);
            if (record == null) return NotFound();

            var doctorUser = await _userManager.GetUserAsync(User);
            if (record.Doctor?.ApplicationUserId != doctorUser?.Id)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xem hồ sơ này.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(record);
        }

        // GET: /Doctor/MedicalRecord/History?patientId=1
        public async Task<IActionResult> History(int patientId)
        {
            // Should verify if the doctor has any appointments with this patient
            // We just fetch it for simplicity, assuming doctor has the right
            var records = await _medicalRecordRepository.GetByPatientIdAsync(patientId);
            ViewBag.PatientId = patientId;
            return View(records);
        }
    }
}
