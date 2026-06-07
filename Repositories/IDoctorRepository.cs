using System.Collections.Generic;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;

namespace HUTECH_Hospital.Repositories
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<IEnumerable<Doctor>> GetActiveAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<IEnumerable<Doctor>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<Doctor>> SearchAsync(string keyword);
        Task AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
    }
}
