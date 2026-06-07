using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.ApplicationUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveAsync()
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.ApplicationUser)
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.ApplicationUser)
                .Include(d => d.DoctorSchedules)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Doctor>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId && d.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return await GetActiveAsync();
            var lowerKeyword = keyword.ToLower();
            
            return await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.IsActive && 
                    (d.FullName!.ToLower().Contains(lowerKeyword) || 
                     d.Specialization!.ToLower().Contains(lowerKeyword)))
                .ToListAsync();
        }

        public async Task AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await GetByIdAsync(id);
            if (doctor != null)
            {
                // Soft delete by default to preserve medical history
                doctor.IsActive = false;
                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
            }
        }
    }
}
