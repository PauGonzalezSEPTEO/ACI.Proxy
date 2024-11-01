using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Models
{
    [PrimaryKey(nameof(TemplateId), nameof(BuildingId))]
    public class TemplateBuilding : IAuditable
    {
        [InverseProperty("TemplatesBuildings")]
        public Building Building { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Building")]
        public int BuildingId { get; set; }

        [InverseProperty("TemplatesBuildings")]
        public Template Template { get; set; }

        [Key]
        [Column(Order = 0)]
        [ForeignKey("Template")]
        public int TemplateId { get; set; }
    }
}
