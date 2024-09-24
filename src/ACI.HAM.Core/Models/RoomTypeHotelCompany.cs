using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(RoomTypeId), nameof(CompanyId), nameof(HotelId), IsUnique = true, Name = "UQ_RoomTypeHotelCompany")]
    public class RoomTypeHotelCompany : IAuditable
    {
        [InverseProperty("RoomTypeHotelsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("RoomTypeHotelsCompanies")]
        public virtual Hotel Hotel { get; set; }

        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("RoomTypeHotelsCompanies")]
        public virtual RoomType RoomType { get; set; }

        [Required]
        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }
    }
}
