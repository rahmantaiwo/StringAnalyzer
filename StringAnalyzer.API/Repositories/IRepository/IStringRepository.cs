using StringAnalyzer.API.Models;

namespace StringAnalyzer.API.Repositories.IRepository
{
    public interface IStringRepository
    {
        Task<IEnumerable<StringRecord>> GetAllAsync();
        Task<StringRecord?> GetByIdAsync(string id);
        Task AddAsync(StringRecord record);
        Task<bool> ExistsAsync(string value);
        Task SaveChangesAsync();
        Task<bool>DeleteAsync(StringRecord record);
    }
}
