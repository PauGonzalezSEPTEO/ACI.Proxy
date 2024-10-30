using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ACI.HAM.Core.Dtos
{
    public class TemplateDto
    {
        private string _content;
        private readonly string _languageCode;
        private string _name;
        private string _shortDescription;

        public TemplateDto() : this(null) { }

        public TemplateDto(string languageCode) => _languageCode = languageCode;

        [Required(ErrorMessage = "Content is required")]
        public string Content
        {
            get
            {
                string localizedContent = null;
                if (!string.IsNullOrEmpty(_languageCode))
                {
                    localizedContent = Translations.SingleOrDefault(x => x.LanguageCode == _languageCode)?.Content;
                }
                return !string.IsNullOrEmpty(localizedContent) ? localizedContent : _content;
            }
            set => _content = value;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name should have at least 4 characters")]
        [MaxLength(256, ErrorMessage = "Name should have maximum 256 characters")]
        public string Name
        {
            get
            {
                string localizedName = null;
                if (!string.IsNullOrEmpty(_languageCode))
                {
                    localizedName = Translations.SingleOrDefault(x => x.LanguageCode == _languageCode)?.Name;
                }
                return !string.IsNullOrEmpty(localizedName) ? localizedName : _name;
            }
            set => _name = value;
        }

        [MaxLength(500, ErrorMessage = "Short description should have maximum 500 characters")]
        public string ShortDescription
        {
            get
            {
                string localizedShortDescription = null;
                if (!string.IsNullOrEmpty(_languageCode))
                {
                    localizedShortDescription = Translations.SingleOrDefault(x => x.LanguageCode == _languageCode)?.ShortDescription;
                }
                return !string.IsNullOrEmpty(localizedShortDescription) ? localizedShortDescription : _shortDescription;
            }
            set => _shortDescription = value;
        }

        internal ICollection<TemplateTranslationDto> Translations { get; set; } = new HashSet<TemplateTranslationDto>();
    }
}
