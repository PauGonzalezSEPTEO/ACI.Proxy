using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Dtos
{
    public class HotelDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name should have at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }

        public string CompanyName { get; set; }
    }
}
