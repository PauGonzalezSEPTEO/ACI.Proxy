using ACI.Base.Mail.Localization;
using ACI.Base.Mail.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Mail.Dtos
{
    public class SendPasswordChangeMailDto
    {
        [Required(ErrorMessage = "Subject is required")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "You must provide at least one email address")]
        [ValidEmailList(ErrorMessageResourceName = "Email_addresses_must_be_between_and_characters", ErrorMessageResourceType = typeof(DataAnnotations), MinLength = 3, MaxLength = 256)]
        public string? To { get; set; }
    }
}
