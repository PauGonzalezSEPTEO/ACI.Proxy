using System.ComponentModel.DataAnnotations;
using System.Linq;
using ACI.HAM.Core.Models;

namespace ACI.HAM.Core.Dtos
{
    public class BuildingDto
    {
        private readonly string _languageCode;
        private string _name;
        private string _shortDescription;

        public BuildingDto() : this(null) { }

        public BuildingDto(string languageCode) => _languageCode = languageCode;

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

        public string HotelName { get; set; }

        public ICollection<RoomTypeDto> RoomTypes
        {
            get
            {
                return RoomTypesBuildings.Select(x => new RoomTypeDto(_languageCode)
                {
                    Name = x.RoomType?.Name,
                    ShortDescription = x.RoomType?.ShortDescription,
                    Translations = x.RoomType.Translations.Select(x => new RoomTypeTranslationDto()
                    {
                        LanguageCode = x.LanguageCode,
                        Name = x.Name,
                        ShortDescription = x.ShortDescription
                    }).ToList()
                }).ToList();
            }
        }

        internal ICollection<RoomTypeBuilding> RoomTypesBuildings { get; set; } = new HashSet<RoomTypeBuilding>();

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

        internal ICollection<BuildingTranslationDto> Translations { get; set; } = new HashSet<BuildingTranslationDto>();
    }
}
