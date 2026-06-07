using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;

namespace HUTECH_Hospital.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<bool> IsDoctorTimeSlotBookedAsync(int doctorId, DateTime date, TimeSpan time, int? excludeAppointmentId = null);
        Task AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(int id);
    }
}
