using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Dtos
{
    public class ProjectEditableDto
    {
        public ProjectEditableDto() { }

        public ProjectEditableDto(ProjectEditableDto source)
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
