using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(TemplateId), nameof(LanguageCode))]
    public class TemplateTranslation : IAuditable
    {
        [Required]
        public string Content { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; }

        [InverseProperty("Translations")]
        public virtual Template Template { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("Template")]
        public int TemplateId { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }
    }
}
