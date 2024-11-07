using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(RoomTypeId), nameof(CompanyId), nameof(ProjectId), IsUnique = true, Name = "UQ_RoomTypeProjectCompany")]
    public class RoomTypeProjectCompany : IAuditable
    {
        [InverseProperty("RoomTypeProjectsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("RoomTypeProjectsCompanies")]
        public virtual Project Project { get; set; }

        [ForeignKey("Project")]
        public int? ProjectId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("RoomTypeProjectsCompanies")]
        public virtual RoomType RoomType { get; set; }

        [Required]
        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }
    }
}
