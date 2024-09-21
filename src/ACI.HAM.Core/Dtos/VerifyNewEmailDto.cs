using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class VerifyNewEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

    }
}
