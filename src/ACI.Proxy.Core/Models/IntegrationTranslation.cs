using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Models
{
    [PrimaryKey(nameof(IntegrationId), nameof(LanguageCode))]
    public class IntegrationTranslation : IAuditable
    {
        public virtual Integration Integration { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("Integration")]
        public int IntegrationId { get; set; }

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
