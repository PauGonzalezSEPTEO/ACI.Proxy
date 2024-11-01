using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(TemplateId), nameof(CompanyId), nameof(HotelId), IsUnique = true, Name = "UQ_TemplateHotelCompany")]
    public class TemplateHotelCompany : IAuditable
    {
        [InverseProperty("TemplateHotelsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("TemplateHotelsCompanies")]
        public virtual Hotel Hotel { get; set; }

        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("TemplateHotelsCompanies")]
        public virtual Template Template { get; set; }

        [Required]
        [ForeignKey("Template")]
        public int TemplateId { get; set; }
    }
}
