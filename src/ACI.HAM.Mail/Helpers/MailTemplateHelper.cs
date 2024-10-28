using ACI.HAM.Mail.Models;
using ACI.HAM.Settings;
using Microsoft.Extensions.Localization;
using RazorEngineCore;
using System.Text;

namespace ACI.HAM.Mail.Helpers
{
    public class MailTemplateHelper
    {
        private readonly IStringLocalizer<Services.MailService> _localizer;
        private readonly UISettings _uiSettings;
        private const string MAIL_TEMPLATE_PATH = "Files/MailTemplates";

        public MailTemplateHelper(IStringLocalizer<Services.MailService> localizer, UISettings uiSettings)
        {
            _localizer = localizer;
            _uiSettings = uiSettings;
        }

        public async Task<string?> LoadMailTemplateAsync<T>(string mailTemplate, T model, CancellationToken cancelationToken = default)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string mailTemplateDirectory = Path.Combine(baseDirectory, MAIL_TEMPLATE_PATH);
                string mailTemplatePath = Path.Combine(mailTemplateDirectory, $"{mailTemplate}.cshtml");
                using (FileStream fileStream = new FileStream(mailTemplatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        string mailTemplateContent = await streamReader.ReadToEndAsync(cancelationToken);
                        streamReader.Close();
                        if (!string.IsNullOrEmpty(mailTemplateContent))
                        {
                            IRazorEngine razorEngine = new RazorEngine();
                            IRazorEngineCompiledTemplate<MailTemplateBaseModel<T>> compiledMailTemplate = await razorEngine.CompileAsync<MailTemplateBaseModel<T>>(mailTemplateContent, builder =>
                            {
                                builder.AddAssemblyReferenceByName("Microsoft.Extensions.Localization.Abstractions");
                            });
                            return await compiledMailTemplate.RunAsync(instance =>
                            {
                                instance.BaseUrl = _uiSettings.BaseUrl;
                                instance.Localizer = _localizer;
                                instance.Model = model;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
