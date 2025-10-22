using Microsoft.EntityFrameworkCore;
using StringAnalyzer.API.Models;
using StringAnalyzer.API.Persistence;
using StringAnalyzer.API.Repositories.IRepository;

namespace StringAnalyzer.API.Repositories.Repository
{
    public class StringRepository(AppDbContext _context) : IStringRepository
    {
        public async Task<IEnumerable<StringRecord>> GetAllAsync()
        {
            return await _context.StringRecords.ToListAsync();
        }

        public async Task<StringRecord?> GetByIdAsync(string id)
        {
            return await _context.StringRecords.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(StringRecord record)
        {
            await _context.StringRecords.AddAsync(record);
        }

        public async Task<bool> ExistsAsync(string value)
        {
            return await _context.StringRecords.AnyAsync(x => x.Value == value);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool>DeleteAsync(StringRecord record)
        {
            _context.StringRecords.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
