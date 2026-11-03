namespace SecurityFootage.Domain.Entities
{
    public class UserLocationAccess
    {
        public long Id {get; set;}
        public long UserId {get; set;}
        public long LocationId {get; set;}
        public User User {get; set;} = null!;
        public Location Location {get; set;} = null!;
    }
}