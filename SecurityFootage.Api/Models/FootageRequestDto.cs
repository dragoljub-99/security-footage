using System.ComponentModel.DataAnnotations;

namespace SecurityFootage.Api.Models
{
    public record FootageRequestDto
    {
        [Required]
        public string Country {get; set;} = null!;
        [Required]
        public string City {get; set;} = null!;
    }
}