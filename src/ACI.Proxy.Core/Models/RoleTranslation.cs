using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(RoleId), nameof(LanguageCode))]
    public class RoleTranslation: IAuditable
    {
        [StringLength(256)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; }

        [InverseProperty("Translations")]        
        public virtual Role Role { get; set; }

        [Key]
        [Column(Order = 0)]
        [Required]
        [ForeignKey("Role")]
        [StringLength(450)]
        public string RoleId { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }
    }
}
