using System.ComponentModel.DataAnnotations;
using ACI.HAM.Core.Attributes;

namespace ACI.HAM.Core.Dtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        [MinLength(2, ErrorMessage = "Firstname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Firstname should have maximum 256 characters")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [MinLength(2, ErrorMessage = "Lastname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Lastname should have maximum 256 characters")]
        public string Lastname { get; set; }

        [MinElements(1, ErrorMessage = "Roles should have at least one element")]
        public ICollection<string> Roles { get; set; }

        public ICollection<UserHotelCompanyDto> UserHotelsCompanies { get; set; }
    }
}
