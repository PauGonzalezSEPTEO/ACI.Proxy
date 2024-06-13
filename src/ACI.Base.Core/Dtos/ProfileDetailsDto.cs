using System.ComponentModel.DataAnnotations;

namespace ACI.Base.Core.Dtos
{
    public class ProfileDetailsDto
    {
        public bool? AllowCommercialMail { get; set; }

        public string Avatar { get; set; }

        public bool? CommunicationByMail { get; set; }

        public bool? CommunicationByPhone { get; set; }

        [MinLength(2)]
        [MaxLength(256)]
        public string CompanyName { get; set; }

        [StringLength(2)]
        public string CountryAlpha2Code { get; set; }

        [StringLength(3)]
        public string CurrencyCode { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(256)]
        public string Firstname { get; set; }

        [StringLength(2)]
        public string LanguageAlpha2Code { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(256)]
        public string Lastname { get; set; }

        [MinLength(4)]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
    }
}
