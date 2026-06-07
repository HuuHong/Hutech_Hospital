using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords
                .Include(m => m.Appointment)
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Include(m => m.Prescription)
                .ThenInclude(p => p.PrescriptionDetails)
                .ThenInclude(pd => pd.Medicine)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetByIdAsync(int id)
        {
            return await _context.MedicalRecords
                .Include(m => m.Appointment)
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Include(m => m.Prescription)
                .ThenInclude(p => p.PrescriptionDetails)
                .ThenInclude(pd => pd.Medicine)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Prescription)
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.MedicalRecords
                .Include(m => m.Patient)
                .Where(m => m.DoctorId == doctorId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.MedicalRecords
                .Include(m => m.Prescription)
                .ThenInclude(p => p.PrescriptionDetails)
                .FirstOrDefaultAsync(m => m.AppointmentId == appointmentId);
        }

        public async Task AddAsync(MedicalRecord record)
        {
            await _context.MedicalRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MedicalRecord record)
        {
            _context.MedicalRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record != null)
            {
                _context.MedicalRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}
