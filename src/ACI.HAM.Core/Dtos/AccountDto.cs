using System.ComponentModel.DataAnnotations;

namespace ACI.HAM.Core.Dtos
{
    public class AccountDto : ProfileDetailsDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string Email { get; set; }

        public bool TwoFactorEnabled { get; set; }
    }
}
