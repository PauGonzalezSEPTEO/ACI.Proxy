using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Models
{
    public class UpdatableUser
    {
        [MinLength(3)]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(256)]
        public string Firstname { get; set; }

        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(256)]
        public string Lastname { get; set; }

        [MinLength(3)]
        [MaxLength(256)]
        public string NormalizedEmail => Email?.ToUpperInvariant();

        [MinLength(3)]
        [MaxLength(256)]
        public string NormalizedUserName => Email?.ToUpperInvariant();

        public virtual ICollection<UserCompany> UserCompanies { get; } = new List<UserCompany>();

        public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
    }
}
