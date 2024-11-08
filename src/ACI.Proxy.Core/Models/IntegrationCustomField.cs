using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Proxy.Core.Models
{
    public class IntegrationCustomField
    {
        [Key]
        public int Id { get; set; }

        public virtual Integration Integration { get; set; }

        [ForeignKey("Integration")]
        public int IntegrationId { get; set; }

        [Required]
        [StringLength(256)]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
