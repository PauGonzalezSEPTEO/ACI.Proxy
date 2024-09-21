using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class BuildingEditableDto
    {
        public BuildingEditableDto() { }

        public BuildingEditableDto(BuildingEditableDto source)
        {
            HotelId = source.HotelId;
            Id = source.Id;
            Name = source.Name;
            RoomTypes = source.RoomTypes;
            ShortDescription = source.ShortDescription;
            Translations = source.Translations;
        }

        [Required(ErrorMessage = "Hotel is required")]
        public int HotelId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name should have at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }

        public ICollection<int> RoomTypes { get; set; } = new HashSet<int>();

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription { get; set; }

        public ICollection<BuildingTranslationDto> Translations { get; set; } = new HashSet<BuildingTranslationDto>();
    }
}
