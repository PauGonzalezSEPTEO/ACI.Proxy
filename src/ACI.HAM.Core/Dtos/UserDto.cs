using System.ComponentModel.DataAnnotations;
using System.Linq;
using ACI.HAM.Core.Models;

namespace ACI.HAM.Core.Dtos
{
    public class UserDto
    {
        private readonly string _languageCode;

        public UserDto() : this(null) { }

        public UserDto(string languageCode) => _languageCode = languageCode;

        public DateTimeOffset? CreateDate { get; set; }

        public ICollection<string> CompanyNames
        {
            get
            {
                return UserHotelsCompanies
                    .Select(x => x.CompanyName)
                    .Distinct()
                    .ToList();
            }
        }

        public ICollection<string> HotelNames
        {
            get
            {
                return UserHotelsCompanies
                    .Where(x => x.HotelName != null)
                    .Select(x => x.HotelName)
                    .Distinct()
                    .ToList();
            }
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        [MinLength(2, ErrorMessage = "Firstname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Firstname should have maximum 256 characters")]
        public string Firstname { get; set; }

        public DateTimeOffset? LastLoginDate { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [MinLength(2, ErrorMessage = "Lastname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Lastname should have maximum 256 characters")]
        public string Lastname { get; set; }

        public ICollection<string> RoleNames
        {
            get
            {
                return Roles.Select(x => x.Name).ToList();
            }
        }

        internal ICollection<RoleDto> Roles { get; set; } = new HashSet<RoleDto>();

        public ICollection<UserHotelCompanyDto> UserHotelsCompanies { get; set; }
    }
}
