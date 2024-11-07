using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.Proxy.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(Id))]
    public class Building : IFilterDto<Building, BuildingDto>, IAuditable
    {
        [InverseProperty("Building")]
        public virtual ICollection<BoardBuilding> BoardsBuildings { get; set; } = new HashSet<BoardBuilding>();

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

        [InverseProperty("Buildings")]
        public virtual Project Project { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(256)]
        public string Name { get; set; }

        [InverseProperty("Building")]
        public virtual ICollection<RoomTypeBuilding> RoomTypesBuildings { get; set; } = new HashSet<RoomTypeBuilding>();

        [StringLength(500)]
        public string ShortDescription { get; set; }

        [InverseProperty("Building")]
        public virtual ICollection<BuildingTranslation> Translations { get; set; } = new HashSet<BuildingTranslation>();

        [InverseProperty("Building")]
        public virtual ICollection<TemplateBuilding> TemplatesBuildings { get; set; } = new HashSet<TemplateBuilding>();
    }
}
