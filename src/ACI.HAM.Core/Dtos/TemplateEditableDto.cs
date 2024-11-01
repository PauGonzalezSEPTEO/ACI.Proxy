using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class TemplateEditableDto
    {
        public TemplateEditableDto() { }

        public TemplateEditableDto(TemplateEditableDto source)
        {
            Content = source.Content;
            Id = source.Id;
            Name = source.Name;
            ShortDescription = source.ShortDescription;
            Translations = source.Translations;
            TemplateHotelsCompanies = source.TemplateHotelsCompanies;
        }

        public ICollection<int> Buildings { get; set; } = new HashSet<int>();

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name should have at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription { get; set; }

        public ICollection<TemplateTranslationDto> Translations { get; set; } = new HashSet<TemplateTranslationDto>();

        public ICollection<TemplateHotelCompanyDto> TemplateHotelsCompanies { get; set; }
    }
}
