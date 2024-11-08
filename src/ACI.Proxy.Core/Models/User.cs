using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.Proxy.Core.Dtos;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Proxy.Core.Models
{
    public class User : IdentityUser,  IFilterDto<User, UserDto>, IAuditable
    {
        public bool? AllowCommercialMail { get; set; }

        public string Avatar { get; set; }

        public bool? CommunicationByMail { get; set; }

        public bool? CommunicationByPhone { get; set; }

        [MinLength(2)]
        [MaxLength(256)]
        public string CompanyName { get; set; }

        [StringLength(2)]
        public string CountryAlpha2Code { get; set; }

        [PersonalData]
        public DateTimeOffset CreateDate { get; set; }

        [StringLength(3)]
        public string CurrencyCode { get; set; }

        public static IQueryable<UserDto> FilterAndOrder(IQueryable<User> query, IMapper mapper, string search, string ordering, string languageCode)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "lastname asc";
            }
            return query
                .ProjectTo<UserDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || (!string.IsNullOrEmpty(x.Lastname) && x.Lastname.Contains(search)))
                .OrderBy(ordering);
        }

        [Required]
        [MinLength(2)]
        [MaxLength(256)]
        public string Firstname { get; set; }

        [StringLength(2)]
        public string LanguageAlpha2Code { get; set; }

        [PersonalData]
        public DateTimeOffset? LastLoginDate { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(256)]
        public string Lastname { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset RefreshTokenExpiryTime { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserApiKey> UserApiKeys { get; } = new List<UserApiKey>();

        [InverseProperty("User")]
        public virtual ICollection<UserApiUsageStatistic> UserApiUsageStatistics { get; } = new List<UserApiUsageStatistic>();

        [InverseProperty("User")]
        public virtual ICollection<UserProjectCompany> UserProjectsCompanies { get; } = new List<UserProjectCompany>();

        public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
    }
}
