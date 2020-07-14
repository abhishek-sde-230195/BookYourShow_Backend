using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.DTO
{
    public class LoginDto
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
