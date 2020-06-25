using System.ComponentModel.DataAnnotations;

namespace BookYourShow.DataTransferObject.DTO {
    public class ForgetPasswordDto {
        [Required]
        [StringLength (50, MinimumLength = 5)]
        public string NewPassword { get; set; }

         [Required]
        [StringLength (50, MinimumLength = 5)]
        [Compare("NewPassword", 
        ErrorMessage = "Confirm password does not match.")]

        public string ConfirmPassword { get; set; }
         
        public string Token {get;set;}

        public string Email{get;set;}
    }
}