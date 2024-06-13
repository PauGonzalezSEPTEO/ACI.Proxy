using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Dtos
{
    public class HotelEditableDto
    {
        public HotelEditableDto() { }

        public HotelEditableDto(HotelEditableDto source)
        {
            Id = source.Id;
            Name = source.Name;
        }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }
    }
}
