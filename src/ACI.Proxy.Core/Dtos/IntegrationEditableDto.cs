using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Dtos
{
    public class IntegrationEditableDto
    {
        public IntegrationEditableDto() { }

        public IntegrationEditableDto(IntegrationEditableDto source)
        {
            CustomFields = source.CustomFields;
            Id = source.Id;
            Name = source.Name;
            ProjectId = source.ProjectId;
            RoomTypes = source.RoomTypes;
            ShortDescription = source.ShortDescription;
            Translations = source.Translations;
        }

        public ICollection<IntegrationCustomFieldDto> CustomFields { get; set; } = new HashSet<IntegrationCustomFieldDto>();

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name should have at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Project is required")]
        public int ProjectId { get; set; }

        public ICollection<int> RoomTypes { get; set; } = new HashSet<int>();

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription { get; set; }

        public ICollection<IntegrationTranslationDto> Translations { get; set; } = new HashSet<IntegrationTranslationDto>();
    }
}
