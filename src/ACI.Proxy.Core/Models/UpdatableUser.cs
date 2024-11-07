using System.ComponentModel.DataAnnotations;

namespace ACI.Proxy.Core.Models
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

        public virtual ICollection<UserProjectCompany> UserProjectsCompanies { get; } = new List<UserProjectCompany>();

        public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
    }
}
