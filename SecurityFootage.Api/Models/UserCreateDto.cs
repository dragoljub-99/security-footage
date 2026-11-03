using System.ComponentModel.DataAnnotations;

namespace SecurityFootage.Api.Models
{
    public record UserCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string NickName {get; set;} = null!;
    }
}