using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class UserEditableDto
    {
        public UserEditableDto() { }

        public UserEditableDto(UserEditableDto source)
        {
            Email = source.Email;
            Firstname = source.Firstname;
            Id = source.Id;
            Lastname = source.Lastname;
            Roles = source.Roles;
            UserHotelsCompanies = source.UserHotelsCompanies;
        }

        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        [MinLength(2, ErrorMessage = "Firstname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Firstname should have maximum 256 characters")]
        public string Firstname { get; set; }

        public string Id { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [MinLength(2, ErrorMessage = "Lastname should have at least 2 characters")]
        [MaxLength(256, ErrorMessage = "Lastname should have maximum 256 characters")]
        public string Lastname { get; set; }

        public ICollection<string> Roles { get; set; }

        public ICollection<UserHotelCompanyDto> UserHotelsCompanies { get; set; }
    }
}
