using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.HAM.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(Id))]
    public class UserApiKey : IFilterDto<UserApiKey, UserApiKeyDto>, IAuditable
    {
        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        [MaxLength(512)]
        public string EncryptedApiKey { get; set; }

        [Required]
        public DateTimeOffset Expiration { get; set; }

        public static IQueryable<UserApiKeyDto> FilterAndOrder(IQueryable<Board> query, IMapper mapper, string search, string ordering, string languageCode = null)
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "expiration desc";
            }
            return query
                .ProjectTo<UserApiKeyDto>(mapper.ConfigurationProvider, new { languageCode })
                .Where(x => string.IsNullOrEmpty(search) || x.Expiration.ToString().Contains(search))
                .OrderBy(ordering);
        }

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
