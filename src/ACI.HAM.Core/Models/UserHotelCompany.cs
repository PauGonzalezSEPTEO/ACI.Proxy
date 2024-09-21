using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(UserId), nameof(CompanyId), nameof(HotelId), IsUnique = true, Name = "UQ_UserCompanyHotel")]
    public class UserHotelCompany : IAuditable
    {
        [InverseProperty("UserHotelsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("UserHotelsCompanies")]
        public virtual Hotel Hotel { get; set; }

        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("UserHotelsCompanies")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(450)]
        [ForeignKey("User")]
        public string UserId { get; set; }
    }
}
