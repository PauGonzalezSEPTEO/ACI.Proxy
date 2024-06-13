using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Dtos
{
    public class RoleEditableDto
    {
        public RoleEditableDto() { }

        public RoleEditableDto(RoleEditableDto source)
        {
            Id = source.Id;
            Name = source.Name;
            ShortDescription = source.ShortDescription; 
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name should have at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription { get; set; }

        public ICollection<RoleTranslationDto> Translations { get; set; } = new HashSet<RoleTranslationDto>();
    }
}

