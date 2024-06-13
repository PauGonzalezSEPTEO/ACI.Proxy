using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Dtos
{
    public class CompanyEditableDto
    {
        public CompanyEditableDto() { }

        public CompanyEditableDto(CompanyEditableDto source)
        {
            Id = source.Id;
            Name = source.Name;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name { get; set; }
    }
}
