using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ACI.Proxy.Core.Dtos
{
    public class IntegrationDto
    {
        private readonly string _languageCode;
        private string _name;
        private string _shortDescription;

        public IntegrationDto() : this(null) { }

        public IntegrationDto(string languageCode) => _languageCode = languageCode;

        internal ICollection<IntegrationCustomFieldDto> CustomFields { get; set; } = new HashSet<IntegrationCustomFieldDto>();

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

        public string ProjectName { get; set; }

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

        internal ICollection<IntegrationTranslationDto> Translations { get; set; } = new HashSet<IntegrationTranslationDto>();
    }
}