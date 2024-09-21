using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using ACI.HAM.Mail.Localization;
using ACI.HAM.Mail.ValidationAttributes;

namespace ACI.HAM.Mail.Dtos
{
    public class SendMailDto
    {
        public List<Attachment>? Attachments { get; set; }

        [ValidEmailList(ErrorMessageResourceName = "Email_addresses_must_be_between_and_characters", ErrorMessageResourceType = typeof(DataAnnotations), MinLength = 3, MaxLength = 256)]
        public string[]? Bcc { get; set; }

        public string? Body { get; set; }

        [ValidEmailList(ErrorMessageResourceName = "Email_addresses_must_be_between_and_characters", ErrorMessageResourceType = typeof(DataAnnotations), MinLength = 3, MaxLength = 256)]
        public string[]? Cc { get; set; }

        public string? DisplayName { get; set; }

        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string? From { get; set; }

        [MinLength(3, ErrorMessage = "Email should have at least 3 characters")]
        [MaxLength(256, ErrorMessage = "Email should have maximum 256 characters")]
        public string? ReplyTo { get; set; }

        public string? ReplyToName { get; set;  }

        [Required(ErrorMessage = "Subject is required")]
        public string? Subject { get; set;  }

        [Required(ErrorMessage = "You must provide at least one email address")]
        [ValidEmailList(ErrorMessageResourceName = "Email_addresses_must_be_between_and_characters", ErrorMessageResourceType = typeof(DataAnnotations), MinLength = 3, MaxLength = 256)]
        public string[]? To { get; set; }
    }
}
