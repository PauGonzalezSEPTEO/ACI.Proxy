using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.Proxy.Core.Dtos;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ACI.Proxy.Core.Models
{
    public class Role : IdentityRole, IFilterDto<Role, RoleDto>, IAuditable
    {
        public Role() : base() { }

        public Role(string name) : base(name) { }

        public static IQueryable<RoleDto> FilterAndOrder(IQueryable<Role> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            return query
                .ProjectTo<RoleDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)))
                .OrderBy(ordering);
        }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<RoleTranslation> Translations { get; set; } = new HashSet<RoleTranslation>();

        public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
    }
}
