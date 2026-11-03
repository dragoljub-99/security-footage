namespace SecurityFootage.Domain.Entities
{
    public class EventQueue
    {
        public long Id {get; set;}
        public long UserId {get; set;}
        public string Nickname {get; set;} = null!;
        public User User {get; set;} = null!;
        public long? LocationId {get; set;}
        public Location? Location {get; set;}
        public QueueProcessingStatus Status {get; set;}
        public DateTime CreatedAtUtc {get; set;} 
    }
}