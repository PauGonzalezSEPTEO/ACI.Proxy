using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.Base.Core.Models
{
    [PrimaryKey(nameof(RoomTypeId), nameof(BuildingId))]
    public class RoomTypeBuilding: IAuditable
    {
        [InverseProperty("RoomTypesBuildings")]        
        public Building Building { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Building")]
        public int BuildingId { get; set; }

        [InverseProperty("RoomTypesBuildings")]        
        public RoomType RoomType { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }
    }
}
