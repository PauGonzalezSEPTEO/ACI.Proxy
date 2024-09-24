using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(BoardId), nameof(CompanyId), nameof(HotelId), IsUnique = true, Name = "UQ_BoardHotelCompany")]
    public class BoardHotelCompany : IAuditable
    {
        [InverseProperty("BoardHotelsCompanies")]
        public virtual Company Company { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [InverseProperty("BoardHotelsCompanies")]
        public virtual Hotel Hotel { get; set; }

        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [InverseProperty("BoardHotelsCompanies")]
        public virtual Board Board { get; set; }

        [Required]
        [ForeignKey("Board")]
        public int BoardId { get; set; }
    }
}
