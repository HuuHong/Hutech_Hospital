using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;

namespace HUTECH_Hospital.Repositories
{
    public interface IDoctorScheduleRepository
    {
        Task<IEnumerable<DoctorSchedule>> GetAllAsync();
        Task<DoctorSchedule?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<DoctorSchedule>> GetByDateAsync(DateTime workDate);
        Task AddAsync(DoctorSchedule schedule);
        Task UpdateAsync(DoctorSchedule schedule);
        Task DeleteAsync(int id);
    }
}
