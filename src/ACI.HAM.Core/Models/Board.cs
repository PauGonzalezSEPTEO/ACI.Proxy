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
    public partial class Board : IFilterDto<Board, BoardDto>, IAuditable
    {
        [Required]
        [MinLength(4)]
        [MaxLength(256)]
        public string Name { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        [Key]
        public int Id { get; set; }

        [InverseProperty("Board")]
        public virtual ICollection<BoardBuilding> BoardsBuildings { get; set; } = new HashSet<BoardBuilding>();

        [InverseProperty("Board")]
        public virtual ICollection<BoardTranslation> Translations { get; set; } = new HashSet<BoardTranslation>();

        public static IQueryable<BoardDto> FilterAndOrder(IQueryable<Board> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<BoardDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }
    }
}
