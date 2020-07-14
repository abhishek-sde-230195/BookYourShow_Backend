using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.DTO
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        [Compare("Password",
            ErrorMessage = "Confirm password does not match.")]

        public string ConfirmPassword { get; set; }
    }
}
