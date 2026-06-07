using System.Collections.Generic;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;

namespace HUTECH_Hospital.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<IEnumerable<Department>> GetActiveAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<IEnumerable<Department>> SearchAsync(string keyword);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
    }
}
