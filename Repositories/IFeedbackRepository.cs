using HUTECH_Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Repositories
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(int id);
        Task<IEnumerable<Feedback>> GetByPatientIdAsync(int patientId);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int id);
    }
}
