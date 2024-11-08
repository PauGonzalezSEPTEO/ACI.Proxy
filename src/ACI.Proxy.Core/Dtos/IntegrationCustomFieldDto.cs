using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Dtos
{
    public class IntegrationCustomFieldDto
    {
        [Required(ErrorMessage = "Key is required")]
        [MaxLength(256, ErrorMessage = "Key should have maximum 256 characters")]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
