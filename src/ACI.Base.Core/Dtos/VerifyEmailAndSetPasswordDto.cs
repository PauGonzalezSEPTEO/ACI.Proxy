using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Dtos
{
    public class VerifyEmailAndSetPasswordDto
    {
        [Required(ErrorMessage = "Confirmation password is required")]
        [MinLength(3, ErrorMessage = "Confirmation password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Confirmation password should have maximum 100 characters")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmationPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email token is required")]
        public string EmailToken { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(3, ErrorMessage = "Password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Password should have maximum 100 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password token is required")]
        public string PasswordToken { get; set; }
    }
}
