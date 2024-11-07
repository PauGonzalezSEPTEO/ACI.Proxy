using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }
    }
}
