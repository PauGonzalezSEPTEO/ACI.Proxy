using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Base.Core.Models
{
    [PrimaryKey(nameof(BoardId), nameof(BuildingId))]
    public class BoardBuilding: IAuditable
    {
        [InverseProperty("BoardsBuildings")]
        public Building Building { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Building")]
        public int BuildingId { get; set; }

        [InverseProperty("BoardsBuildings")]
        public Board Board { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("Board")]
        public int BoardId { get; set; }
    }
}
