using HUTECH_Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Repositories
{
    public interface IMedicalRecordRepository
    {
        Task<IEnumerable<MedicalRecord>> GetAllAsync();
        Task<MedicalRecord?> GetByIdAsync(int id);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId);
        Task<MedicalRecord?> GetByAppointmentIdAsync(int appointmentId);
        Task AddAsync(MedicalRecord record);
        Task UpdateAsync(MedicalRecord record);
        Task DeleteAsync(int id);
    }
}
