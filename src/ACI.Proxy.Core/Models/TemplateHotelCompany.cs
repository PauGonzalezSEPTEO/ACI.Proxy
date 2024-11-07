using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(TemplateId), nameof(CompanyId), nameof(ProjectId), IsUnique = true, Name = "UQ_TemplateProjectCompany")]
    public class TemplateProjectCompany : IAuditable
    {
        [InverseProperty("TemplateProjectsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("TemplateProjectsCompanies")]
        public virtual Project Project { get; set; }

        [ForeignKey("Project")]
        public int? ProjectId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("TemplateProjectsCompanies")]
        public virtual Template Template { get; set; }

        [Required]
        [ForeignKey("Template")]
        public int TemplateId { get; set; }
    }
}
