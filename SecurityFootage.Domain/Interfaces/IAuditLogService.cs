using SecurityFootage.Domain.Entities;

namespace SecurityFootage.Domain.Interfaces
{
    public interface IAuditLogService
    {
         void AddAudit(AuditLog log);
    }
}