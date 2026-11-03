using SecurityFootage.Domain.Entities;
using SecurityFootage.Infrastructure.Persistence;
using SecurityFootage.Domain.Interfaces;

namespace SecurityFootage.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly SecurityFootageDbContext _dbContext;

        public AuditLogService(SecurityFootageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddAudit(AuditLog log)
        {
            _dbContext.AuditLogs.Add(log);
        }
    }
}