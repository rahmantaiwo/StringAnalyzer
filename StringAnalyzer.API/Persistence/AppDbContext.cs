using Microsoft.EntityFrameworkCore;
using StringAnalyzer.API.Models;

namespace StringAnalyzer.API.Persistence
{
    public class AppDbContext : DbContext  // ✅ Must inherit from DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<StringRecord> StringRecords { get; set; }
       
    }
}
