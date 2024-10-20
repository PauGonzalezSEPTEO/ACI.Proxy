using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class TemplateTranslationDto
    {
        [Required(ErrorMessage = "Language code is required")]
        [MaxLength(10, ErrorMessage = "Language code should have maximum 10 characters")]
        public string LanguageCode { get; set; }

        [StringLength(256, MinimumLength = 4, ErrorMessage = "Name should have between 4 and 256 characters")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription { get; set; }
    }
}
