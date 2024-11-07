using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Dtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        [MinLength(6, ErrorMessage = "Current password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Current password should have maximum 100 characters")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New confirmation password is required")]
        [MinLength(6, ErrorMessage = "New confirmation password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "New confirmation password should have maximum 100 characters")]
        [Compare("NewPassword", ErrorMessage = "The password and new confirmation password do not match")]
        public string NewConfirmationPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "New password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "New password should have maximum 100 characters")]
        public string NewPassword { get; set; }
    }
}
