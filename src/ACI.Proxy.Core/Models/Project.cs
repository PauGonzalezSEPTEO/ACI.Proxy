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
    public class Project : IFilterDto<Project, ProjectDto>, IAuditable
    {
        [InverseProperty("Project")]
        public virtual ICollection<BoardProjectCompany> BoardProjectsCompanies { get; } = new List<BoardProjectCompany>();

        [InverseProperty("Project")]
        public virtual ICollection<Building> Buildings { get; set; } = new HashSet<Building>();

        [InverseProperty("Projects")]        
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Key]
        public int Id { get; set; }

        public static IQueryable<ProjectDto> FilterAndOrder(IQueryable<Project> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<ProjectDto>(mapper.ConfigurationProvider)
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }

        [InverseProperty("Project")]
        public virtual ICollection<Integration> Integrations { get; set; } = new HashSet<Integration>();

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [InverseProperty("Project")]
        public virtual ICollection<RoomTypeProjectCompany> RoomTypeProjectsCompanies { get; } = new List<RoomTypeProjectCompany>();

        [InverseProperty("Project")]
        public virtual ICollection<TemplateProjectCompany> TemplateProjectsCompanies { get; } = new List<TemplateProjectCompany>();

        [InverseProperty("Project")]
        public virtual ICollection<UserProjectCompany> UserProjectsCompanies { get; } = new List<UserProjectCompany>();
    }
}
