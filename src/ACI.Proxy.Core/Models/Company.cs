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
    public class Company : IFilterDto<Company, CompanyDto>, IAuditable
    {
        [InverseProperty("Company")]
        public virtual ICollection<BoardProjectCompany> BoardProjectsCompanies { get; } = new List<BoardProjectCompany>();

        [InverseProperty("Company")]
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        [Key]
        public int Id { get; set; }

        public static IQueryable<CompanyDto> FilterAndOrder(IQueryable<Company> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<CompanyDto>(mapper.ConfigurationProvider)
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [InverseProperty("Company")]
        public virtual ICollection<RoomTypeProjectCompany> RoomTypeProjectsCompanies { get; } = new List<RoomTypeProjectCompany>();

        [InverseProperty("Company")]
        public virtual ICollection<TemplateProjectCompany> TemplateProjectsCompanies { get; } = new List<TemplateProjectCompany>();

        [InverseProperty("Company")]
        public virtual ICollection<UserProjectCompany> UserProjectsCompanies { get; } = new List<UserProjectCompany>();
    }
}
