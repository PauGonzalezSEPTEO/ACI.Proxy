using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(RoomTypeId), nameof(LanguageCode))]
    public class RoomTypeTranslation: IAuditable
    {
        [StringLength(256)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; }

        [InverseProperty("Translations")]       
        public virtual RoomType RoomType { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }
    }
}
