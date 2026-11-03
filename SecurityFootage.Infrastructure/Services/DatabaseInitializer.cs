using Microsoft.EntityFrameworkCore;
using SecurityFootage.Infrastructure.Persistence;
using SecurityFootage.Infrastructure.Abstractions;

namespace SecurityFootage.Infrastructure.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly SecurityFootageDbContext _dbContext;

        public DatabaseInitializer(SecurityFootageDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task EnsureDatabaseSchemaAsync(CancellationToken ct = default)
        {       
                await _dbContext.Database.MigrateAsync(ct);        
        }
    }
}