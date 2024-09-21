using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class RegistrationDto
    {
        [Required(ErrorMessage = "Confirmation password is required")]
        [MinLength(6, ErrorMessage = "Confirmation password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Confirmation password should have maximum 100 characters")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmationPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        [MinLength(2, ErrorMessage = "Firstname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Firstname should have maximum 256 characters")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [MinLength(2, ErrorMessage = "Lastname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Lastname should have maximum 256 characters")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Password should have maximum 100 characters")]
        public string Password { get; set; }
    }
}
