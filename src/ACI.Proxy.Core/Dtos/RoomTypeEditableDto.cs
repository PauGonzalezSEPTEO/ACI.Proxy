using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Dtos
{
    public class RoomTypeEditableDto
    {
        public RoomTypeEditableDto() { }

        public RoomTypeEditableDto(RoomTypeEditableDto source)
        {
            RoomTypeProjectsCompanies = source.RoomTypeProjectsCompanies;
            Buildings = source.Buildings;
            Id = source.Id;
            Name = source.Name;
            ShortDescription = source.ShortDescription;
            Translations = source.Translations;
        }

        public ICollection<int> Buildings { get; set; } = new HashSet<int>();

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name should have at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }

        public ICollection<RoomTypeProjectCompanyDto> RoomTypeProjectsCompanies { get; set; }

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription { get; set; }

        public ICollection<RoomTypeTranslationDto> Translations { get; set; } = new HashSet<RoomTypeTranslationDto>();
    }
}
