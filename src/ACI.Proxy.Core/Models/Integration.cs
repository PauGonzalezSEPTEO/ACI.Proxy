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
    public abstract class Integration : IFilterDto<Integration, IntegrationDto>, IAuditable
    {
        [InverseProperty("Integration")]
        public virtual ICollection<IntegrationCustomField> CustomFields { get; set; } = new HashSet<IntegrationCustomField>();

        public static IQueryable<IntegrationDto> FilterAndOrder(IQueryable<Integration> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<IntegrationDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int IntegrationType { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(256)]
        public string Name { get; set; }

        [InverseProperty("Integrations")]
        public virtual Project Project { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        [InverseProperty("Integration")]
        public virtual ICollection<IntegrationTranslation> Translations { get; set; } = new HashSet<IntegrationTranslation>();
    }
}
