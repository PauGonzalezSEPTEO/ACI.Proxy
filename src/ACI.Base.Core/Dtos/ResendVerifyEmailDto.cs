using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Dtos
{
    public class ResendVerifyEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }
    }
}
