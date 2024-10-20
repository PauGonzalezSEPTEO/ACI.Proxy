using ACI.HAM.Mail.Helpers;
using ACI.HAM.Mail.Models;

namespace ACI.HAM.Mail.Services
{
    public interface ITemplateService
    {
        Task<string> GetTemplateAsync(string name, CancellationToken cancelationToken = default);
    }

    public class TemplateService : ITemplateService
    {
        private readonly MailTemplateHelper _mailTemplateHelper;

        public TemplateService(MailTemplateHelper mailTemplateHelper)
        {
            _mailTemplateHelper = mailTemplateHelper;
        }

        public async Task<string> GetTemplateAsync(string name, CancellationToken cancelationToken = default)
        {
            ChangeEmail changeEmail = new ChangeEmail()
            {
                Url = "localhost"
            };
            var template = await _mailTemplateHelper.LoadMailTemplateAsync(name, changeEmail, cancelationToken);
            return template ?? string.Empty;
        }
    }
}
