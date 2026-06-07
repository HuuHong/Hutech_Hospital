using HUTECH_Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Repositories
{
    public interface IMedicineRepository
    {
        Task<IEnumerable<Medicine>> GetAllAsync();
        Task<IEnumerable<Medicine>> GetActiveAsync();
        Task<Medicine?> GetByIdAsync(int id);
        Task AddAsync(Medicine medicine);
        Task UpdateAsync(Medicine medicine);
        Task DeleteAsync(int id);
        Task<IEnumerable<Medicine>> SearchAsync(string keyword);
    }
}
