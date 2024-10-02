using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(Id))]
    public class UserApiKey : IAuditable
    {
        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        [MaxLength(512)]
        public string EncryptedApiKey { get; set; }

        [Required]
        public DateTimeOffset Expiration { get; set; }

        [Required]
        [MaxLength(64)]
        public string HashedApiKey { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [MaxLength(32)]
        public string Salt { get; set; }

        [InverseProperty("UserApiKeys")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(450)]
        [ForeignKey("User")]
        public string UserId { get; set; }
    }
}
