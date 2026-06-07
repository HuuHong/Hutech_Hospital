using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetActiveAsync()
        {
            return await _context.Departments.Where(d => d.IsActive).ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<IEnumerable<Department>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return await GetActiveAsync();
            return await _context.Departments
                .Where(d => d.IsActive && d.Name.Contains(keyword))
                .ToListAsync();
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetByIdAsync(id);
            if (department != null)
            {
                // Soft delete only setting active to false if there are doctors
                bool hasDoctors = await _context.Doctors.AnyAsync(d => d.DepartmentId == id);
                if (hasDoctors)
                {
                    department.IsActive = false;
                    _context.Departments.Update(department);
                }
                else
                {
                    _context.Departments.Remove(department);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
