using System.ComponentModel.DataAnnotations;

namespace BookYourShow.DataTransferObject.DTO {
    public class RegisterDto {
        [Required]
        [StringLength (50)]
        [EmailAddress]
        public string Email { get; set; }

        public string FullName { get; set; }

        [Required]
        [StringLength (50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required]
        [StringLength (50, MinimumLength = 5)]
        [Compare("Password", 
            ErrorMessage = "Confirm password does not match.")]

        public string ConfirmPassword { get; set; }
    }
}