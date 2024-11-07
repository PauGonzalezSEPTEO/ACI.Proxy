using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(BoardId), nameof(CompanyId), nameof(ProjectId), IsUnique = true, Name = "UQ_BoardProjectCompany")]
    public class BoardProjectCompany : IAuditable
    {
        [InverseProperty("BoardProjectsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("BoardProjectsCompanies")]
        public virtual Project Project { get; set; }

        [ForeignKey("Project")]
        public int? ProjectId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("BoardProjectsCompanies")]
        public virtual Board Board { get; set; }

        [Required]
        [ForeignKey("Board")]
        public int BoardId { get; set; }
    }
}
