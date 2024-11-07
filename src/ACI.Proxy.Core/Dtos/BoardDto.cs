using System.ComponentModel.DataAnnotations;
using System.Linq;
using ACI.Proxy.Core.Models;

namespace ACI.Proxy.Core.Dtos
{
    public class BoardDto
    {
        private readonly string _languageCode;
        private string _name;
        private string _shortDescription;

        public BoardDto() : this(null) { }

        public BoardDto(string languageCode) => _languageCode = languageCode;

        public ICollection<BuildingDto> Buildings
        {
            get
            {
                return BoardsBuildings.Select(x => new BuildingDto(_languageCode)
                {
                    Name = x.Building?.Name,
                    ShortDescription = x.Building?.ShortDescription,
                    Translations = x.Building.Translations.Select(x => new BuildingTranslationDto()
                    {
                        LanguageCode = x.LanguageCode,
                        Name = x.Name,
                        ShortDescription = x.ShortDescription
                    }).ToList()
                }).ToList();
            }
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

        internal ICollection<BoardBuilding> BoardsBuildings { get; set; } = new HashSet<BoardBuilding>();

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

        internal ICollection<BoardTranslationDto> Translations { get; set; } = new HashSet<BoardTranslationDto>();
    }
}
