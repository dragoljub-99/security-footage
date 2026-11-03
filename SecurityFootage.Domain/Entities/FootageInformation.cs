namespace SecurityFootage.Domain.Entities
{
    public class FootageInformation
    {
        public long Id {get; set;}
        public string BlobName {get; set;} = null!;
        public long LocationId {get; set;}
        public Location Location {get; set;} = null!;

    }
}