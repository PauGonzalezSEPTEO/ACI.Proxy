using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class BoardEditableDto
    {
        public BoardEditableDto() { }

        public BoardEditableDto(BoardEditableDto source)
        {
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

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription { get; set; }

        public ICollection<BoardTranslationDto> Translations { get; set; } = new HashSet<BoardTranslationDto>();
    }
}
