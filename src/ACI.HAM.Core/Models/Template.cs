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
    public partial class Template : IFilterDto<Template, TemplateDto>, IAuditable
    {
        [Required]
        public string Content { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(256)]
        public string Name { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        [Key]
        public int Id { get; set; }

        public static IQueryable<TemplateDto> FilterAndOrder(IQueryable<Template> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<TemplateDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }

        [InverseProperty("Template")]
        public virtual ICollection<TemplateTranslation> Translations { get; set; } = new HashSet<TemplateTranslation>();
    }
}
