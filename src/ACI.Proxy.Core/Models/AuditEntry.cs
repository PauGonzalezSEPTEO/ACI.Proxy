using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ACI.Proxy.Core.Models
{
    public enum ActionTypes : ushort
    {
        Insert = 1,
        Update = 2,
        Delete = 3
    }

    [PrimaryKey(nameof(Id))]
    public class AuditEntry
    {
        [Required]
        public ActionTypes ActionType { get; set; }

        [Required]
        public string EntityName { get; set; }

        [Key]
        public int Id { get; set; }

        [Required]
        public string KeyValues { get; set; }

        public string NewValues { get; set; }

        public string OldValues { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [MaxLength(256)]
        public string UserName { get; set; }
    }
}
