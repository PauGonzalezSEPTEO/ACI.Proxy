using ACI.Proxy.Mail.Services;
using Microsoft.Extensions.Localization;
using RazorEngineCore;

namespace ACI.Proxy.Mail.Models
{
    public class MailTemplateBaseModel<T> : RazorEngineTemplateBase
    {
        public string? BaseUrl { get; set; }

        public IStringLocalizer<MailService>? Localizer;

        public new T Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
            }
        }        
    }
}
