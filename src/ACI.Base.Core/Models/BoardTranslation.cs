using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Base.Core.Models
{
    [PrimaryKey(nameof(BoardId), nameof(LanguageCode))]
    public class BoardTranslation: IAuditable
    {
        [StringLength(256)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; }

        [InverseProperty("Translations")]
        public virtual Board Board { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("Board")]
        public int BoardId { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }
    }
}
