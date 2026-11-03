namespace SecurityFootage.Domain.Entities
{
    public class User
    {
        public long Id {get; set;}
        public string Nickname {get; set;} = null!;
        public DateTime CreatedAtUtc {get; set;}
        public ICollection<UserLocationAccess> LocationsAccess {get; set;} = new List<UserLocationAccess>();
        public ICollection<EventQueue> EventQueues {get; set;} = new List<EventQueue>();
        public ICollection<AuditLog> AuditLogs {get; set;} = new List<AuditLog>();
    }

}