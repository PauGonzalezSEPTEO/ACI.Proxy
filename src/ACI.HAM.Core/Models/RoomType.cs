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
    public class RoomType : IFilterDto<RoomType, RoomTypeDto>, IAuditable
    {
        public static IQueryable<RoomTypeDto> FilterAndOrder(IQueryable<RoomType> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<RoomTypeDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }

        [Required]
        [MinLength(4)]
        [MaxLength(256)]
        public string Name { get; set; }

        [Key]
        public int Id { get; set; }

        [InverseProperty("RoomType")]
        public virtual ICollection<RoomTypeHotelCompany> RoomTypeHotelsCompanies { get; } = new List<RoomTypeHotelCompany>();

        [InverseProperty("RoomType")]
        public virtual ICollection<RoomTypeBuilding> RoomTypesBuildings { get; set; } = new HashSet<RoomTypeBuilding>();

        [StringLength(500)]
        public string ShortDescription { get; set; }

        [InverseProperty("RoomType")]
        public virtual ICollection<RoomTypeTranslation> Translations { get; set; } = new HashSet<RoomTypeTranslation>();
    }
}
