using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(UserId), nameof(CompanyId))]
    public class UserCompany : IAuditable
    {
        [InverseProperty("Users")]
        public virtual Company Company { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("Companies")]
        public virtual User User { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("User")]
        public string UserId { get; set; }
    }
}
