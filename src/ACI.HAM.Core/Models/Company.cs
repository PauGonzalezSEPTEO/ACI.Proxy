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
    public class Company : IFilterDto<Company, CompanyDto>, IAuditable
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [InverseProperty("Company")]
        public virtual ICollection<Hotel> Hotels { get; set; } = new HashSet<Hotel>();

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

        [InverseProperty("Company")]
        public virtual ICollection<UserHotelCompany> UserHotelsCompanies { get; } = new List<UserHotelCompany>();
    }
}
