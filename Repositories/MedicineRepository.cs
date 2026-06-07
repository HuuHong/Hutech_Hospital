using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Medicine>> GetAllAsync()
        {
            return await _context.Medicines.OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<IEnumerable<Medicine>> GetActiveAsync()
        {
            return await _context.Medicines.Where(m => m.IsActive).OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<Medicine?> GetByIdAsync(int id)
        {
            return await _context.Medicines.FindAsync(id);
        }

        public async Task AddAsync(Medicine medicine)
        {
            await _context.Medicines.AddAsync(medicine);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Medicine medicine)
        {
            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var medicine = await _context.Medicines.Include(m => m.PrescriptionDetails).FirstOrDefaultAsync(m => m.Id == id);
            if (medicine != null)
            {
                if (medicine.PrescriptionDetails.Any())
                {
                    medicine.IsActive = false;
                    _context.Medicines.Update(medicine);
                }
                else
                {
                    _context.Medicines.Remove(medicine);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Medicine>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetActiveAsync();

            keyword = keyword.ToLower();
            return await _context.Medicines
                .Where(m => m.IsActive && (m.Name.ToLower().Contains(keyword) || (m.Description != null && m.Description.ToLower().Contains(keyword))))
                .ToListAsync();
        }
    }
}
