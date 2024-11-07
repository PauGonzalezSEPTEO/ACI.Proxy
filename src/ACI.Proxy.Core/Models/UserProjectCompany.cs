using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(UserId), nameof(CompanyId), nameof(ProjectId), IsUnique = true, Name = "UQ_UserProjectCompany")]
    public class UserProjectCompany : IAuditable
    {
        [InverseProperty("UserProjectsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("UserProjectsCompanies")]
        public virtual Project Project { get; set; }

        [ForeignKey("Project")]
        public int? ProjectId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("UserProjectsCompanies")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(450)]
        [ForeignKey("User")]
        public string UserId { get; set; }
    }
}
