namespace SecurityFootage.Domain.Entities
{
    public class AuditLog
    {
        public long Id {get; set;}
        public long? UserId {get; set;}
        public User? User {get; set;}
        public long? LocationId {get; set;}
        public Location? Location {get; set;}
        public string? RequestedCountry {get; set;} 
        public string? RequestedCity {get; set;}
        public AuditAction Action {get; set;} 
        public AuditOutcome Result {get; set;}
        public string? Nickname {get; set;}
        public DateTime CreatedAtUtc {get; set;}
    }
}