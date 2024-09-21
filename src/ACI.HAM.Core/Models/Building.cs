using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.HAM.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(Id))]
    public class Building : IFilterDto<Building, BuildingDto>, IAuditable
    {
        [InverseProperty("Building")]
        public virtual ICollection<BoardBuilding> BoardsBuildings { get; set; } = new HashSet<BoardBuilding>();

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(256)]
        public string Name { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        [InverseProperty("Buildings")]
        public virtual Hotel Hotel { get; set; }

        [Required]
        [ForeignKey("Hotel")]
        public int HotelId { get; set; }

        [InverseProperty("Building")]
        public virtual ICollection<RoomTypeBuilding> RoomTypesBuildings { get; set; } = new HashSet<RoomTypeBuilding>();

        [InverseProperty("Building")]
        public virtual ICollection<BuildingTranslation> Translations { get; set; } = new HashSet<BuildingTranslation>();

        public static IQueryable<BuildingDto> FilterAndOrder(IQueryable<Building> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<BuildingDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }
    }
}
