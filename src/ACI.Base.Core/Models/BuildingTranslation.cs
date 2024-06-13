using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Base.Core.Models
{
    [PrimaryKey(nameof(BuildingId), nameof(LanguageCode))]
    public class BuildingTranslation: IAuditable
    {        
        public virtual Building Building { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("Building")]
        public int BuildingId { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }
    }
}
