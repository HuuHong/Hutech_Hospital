using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Repositories
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorSchedule>> GetAllAsync()
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .ThenInclude(d => d.Department)
                .OrderByDescending(s => s.WorkDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<DoctorSchedule?> GetByIdAsync(int id)
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .Where(s => s.DoctorId == doctorId)
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorSchedule>> GetByDateAsync(DateTime workDate)
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .Where(s => s.WorkDate.Date == workDate.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task AddAsync(DoctorSchedule schedule)
        {
            await _context.DoctorSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DoctorSchedule schedule)
        {
            _context.DoctorSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await GetByIdAsync(id);
            if (schedule != null)
            {
                _context.DoctorSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }
    }
}
