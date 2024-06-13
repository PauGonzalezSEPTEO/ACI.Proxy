using System.ComponentModel.DataAnnotations;
using ACI.Base.Mail.Localization;
using ACI.Base.Mail.ValidationAttributes;

namespace ACI.Base.Mail.Dtos
{
    public class SendTwoFactorMailDto
    {
        [Required(ErrorMessage = "Subject is required")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "You must provide at least one email address")]
        [ValidEmailList(ErrorMessageResourceName = "Email_addresses_must_be_between_and_characters", ErrorMessageResourceType = typeof(DataAnnotations), MinLength = 3, MaxLength = 256)]
        public string? To { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string? Token { get; set; }
    }
}
