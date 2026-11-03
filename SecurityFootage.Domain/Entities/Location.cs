using System.Dynamic;

namespace SecurityFootage.Domain.Entities
{
    public class Location
    {
        public long Id {get; set;}
        public string Country {get; set;} = null!;
        public string City {get; set;} = null!;
        public ICollection<UserLocationAccess> UserAccesses {get; set;} = new List<UserLocationAccess>();
        public ICollection<FootageInformation> FootagesInfo {get; set;} = new List<FootageInformation>();

    }
}