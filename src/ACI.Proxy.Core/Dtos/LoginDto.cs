using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password should have at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Password should have maximum 100 characters")]
        public string Password { get; set; }
    }
}
