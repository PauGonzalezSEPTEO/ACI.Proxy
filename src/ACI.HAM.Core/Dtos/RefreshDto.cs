using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class RefreshDto
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}
