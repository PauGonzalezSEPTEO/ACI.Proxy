using ACI.HAM.Mail.Dtos;
using ACI.HAM.Mail.Models;
using ACI.HAM.Mail.Settings;
using ACI.HAM.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorEngineCore;
using System.Text;

namespace ACI.HAM.Mail.Services
{
    public interface IMailService
    {
        Task<bool> SendChangeEmailMailAsync(SendChangeEmailMailDto sendChangeEmailMailDto, CancellationToken cancelationToken = default);

        Task<bool> SendLockoutMailAsync(SendLockoutMailDto sendLockoutMailDto, CancellationToken cancelationToken = default);

        Task<bool> SendMailAsync(SendMailDto sendMailDto, CancellationToken cancelationToken = default);

        Task<bool> SendPasswordChangeMailAsync(SendPasswordChangeMailDto sendPasswordChangeMailDto, CancellationToken cancelationToken = default);

        Task<bool> SendPasswordResetMailAsync(SendPasswordResetMailDto sendPasswordResetMailDto, CancellationToken cancelationToken = default);

        Task<bool> SendTwoFactorMailAsync(SendTwoFactorMailDto sendTwoFactorMailDto, CancellationToken cancelationToken = default);

        Task<bool> SendVerifyEmailMailAsync(SendVerifyEmailMailDto sendVerifyEmailMailDto, CancellationToken cancelationToken = default);
    }

    public class MailService : IMailService
    {
        const string CHANGE_EMAIL_TEMPLATE = "ChangeEmail";
        const string LOCKOUT_TEMPLATE = "Lockout";
        const string MAIL_TEMPLATE_PATH = "Files/MailTemplates";
        const string PASSWORD_CHANGE_TEMPLATE = "PasswordChange";
        const string PASSWORD_RESET_TEMPLATE = "PasswordReset";
        const string TWO_FACTOR_TEMPLATE = "TwoFactor";
        const string VERIFY_EMAIL_TEMPLATE = "VerifyEmail";

        private readonly IStringLocalizer<MailService> _localizer;
        private readonly MailSettings _mailSettings;
        private readonly UISettings _uiSettings;

        public MailService(IOptions<MailSettings> mailSettings, IStringLocalizer<MailService> localizer, IOptions<UISettings> uiSettings)
        {
            _mailSettings = mailSettings.Value;
            _uiSettings = uiSettings.Value;
            _localizer = localizer;
        }

        private async Task<string?> LoadMailTemplateAsync<T>(string mailTemplate, T model, CancellationToken cancelationToken = default)
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
            catch
            {
            }
            return null;
        }

        public async Task<bool> SendChangeEmailMailAsync(SendChangeEmailMailDto sendChangeEmailMailDto, CancellationToken cancelationToken = default)
        {
            if ((sendChangeEmailMailDto != null) && (sendChangeEmailMailDto.To != null))
            {
                ChangeEmail changeEmail = new ChangeEmail()
                {
                    Url = sendChangeEmailMailDto.Url
                };
                string? template = await LoadMailTemplateAsync(CHANGE_EMAIL_TEMPLATE, changeEmail, cancelationToken);
                if (!string.IsNullOrEmpty(template))
                {
                    SendMailDto sendMailDto = new SendMailDto()
                    {
                        Body = template,
                        To = new string[] { sendChangeEmailMailDto.To },
                        Subject = sendChangeEmailMailDto.Subject
                    };
                    return await SendMailAsync(sendMailDto);
                }
            }
            return false;
        }

        public async Task<bool> SendLockoutMailAsync(SendLockoutMailDto sendLockoutMailDto, CancellationToken cancelationToken = default)
        {
            if ((sendLockoutMailDto != null) && (sendLockoutMailDto.To != null))
            {
                Lockout lockout = new Lockout()
                {
                    Url = sendLockoutMailDto.Url
                };
                string? template = await LoadMailTemplateAsync(LOCKOUT_TEMPLATE, lockout, cancelationToken);
                if (!string.IsNullOrEmpty(template))
                {
                    SendMailDto sendMailDto = new SendMailDto()
                    {
                        Body = template,
                        To = new string[] { sendLockoutMailDto.To },
                        Subject = sendLockoutMailDto.Subject
                    };
                    return await SendMailAsync(sendMailDto);
                }
            }
            return false;
        }

        public async Task<bool> SendMailAsync(SendMailDto sendMailDto, CancellationToken cancelationToken = default)
        {
            try
            {
                MimeMessage mail = new MimeMessage();
                mail.From.Add(new MailboxAddress(_mailSettings.DisplayName, sendMailDto.From ?? _mailSettings.From));
                mail.Sender = new MailboxAddress(sendMailDto.DisplayName ?? _mailSettings.DisplayName, sendMailDto.From ?? _mailSettings.From);
                if (sendMailDto.To != null)
                {
                    foreach (string mailAddress in sendMailDto.To.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        mail.To.Add(MailboxAddress.Parse(mailAddress));
                    }
                }
                if (!string.IsNullOrEmpty(sendMailDto.ReplyTo))
                {
                    mail.ReplyTo.Add(new MailboxAddress(sendMailDto.ReplyToName, sendMailDto.ReplyTo));
                }
                if (sendMailDto.Bcc != null)
                {
                    foreach (string mailAddress in sendMailDto.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                    }
                }
                if (sendMailDto.Cc != null)
                {
                    foreach (string mailAddress in sendMailDto.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                    }
                }
                mail.Subject = sendMailDto.Subject;
                BodyBuilder body = new BodyBuilder();
                if (sendMailDto.Attachments != null)
                {
                    foreach (var attachment in sendMailDto.Attachments)
                    {
                        await body.Attachments.AddAsync(attachment.Name, attachment.ContentStream);
                    }
                }
                body.HtmlBody = sendMailDto.Body;
                mail.Body = body.ToMessageBody();
                using (var smtp = new SmtpClient())
                {
                    if (_mailSettings.UseSsl)
                    {
                        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect, cancelationToken);
                    }
                    else if (_mailSettings.UseStartTls)
                    {
                        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls, cancelationToken);
                    }
                    await smtp.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password, cancelationToken);
                    await smtp.SendAsync(mail, cancelationToken);
                    await smtp.DisconnectAsync(true, cancelationToken);
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public async Task<bool> SendPasswordChangeMailAsync(SendPasswordChangeMailDto sendPasswordChangeMailDto, CancellationToken cancelationToken = default)
        {
            if ((sendPasswordChangeMailDto != null) && (sendPasswordChangeMailDto.To != null))
            {
                string? template = await LoadMailTemplateAsync(PASSWORD_CHANGE_TEMPLATE, Type.Missing, cancelationToken);
                if (!string.IsNullOrEmpty(template))
                {
                    SendMailDto sendMailDto = new SendMailDto()
                    {
                        Body = template,
                        To = new string[] { sendPasswordChangeMailDto.To },
                        Subject = sendPasswordChangeMailDto.Subject
                    };
                    return await SendMailAsync(sendMailDto);
                }
            }
            return false;
        }

        public async Task<bool> SendPasswordResetMailAsync(SendPasswordResetMailDto sendPasswordResetMailDto, CancellationToken cancelationToken = default)
        {
            PasswordReset passwordReset = new PasswordReset()
            {
                Url = sendPasswordResetMailDto.Url
            };
            string? template = await LoadMailTemplateAsync(PASSWORD_RESET_TEMPLATE, passwordReset, cancelationToken);
            if (!string.IsNullOrEmpty(template) && !string.IsNullOrEmpty(sendPasswordResetMailDto.To))
            {
                SendMailDto sendMailDto = new SendMailDto()
                {
                    Body = template,
                    To = new string[] { sendPasswordResetMailDto.To },
                    Subject = sendPasswordResetMailDto.Subject
                };
                return await SendMailAsync(sendMailDto);
            }
            return false;
        }

        public async Task<bool> SendTwoFactorMailAsync(SendTwoFactorMailDto sendTwoFactorMailDto, CancellationToken cancelationToken = default)
        {
            if ((sendTwoFactorMailDto != null) && (sendTwoFactorMailDto.To != null))
            {
                TwoFactor twoFactor = new TwoFactor()
                {
                    Token = sendTwoFactorMailDto.Token
                };
                string? template = await LoadMailTemplateAsync(TWO_FACTOR_TEMPLATE, twoFactor, cancelationToken);
                if (!string.IsNullOrEmpty(template))
                {
                    SendMailDto sendMailDto = new SendMailDto()
                    {
                        Body = template,
                        To = new string[] { sendTwoFactorMailDto.To },
                        Subject = sendTwoFactorMailDto.Subject
                    };
                    return await SendMailAsync(sendMailDto);
                }
            }
            return false;
        }

        public async Task<bool> SendVerifyEmailMailAsync(SendVerifyEmailMailDto sendVerifyEmailMailDto, CancellationToken cancelationToken = default)
        {
            VerifyEmail verifyEmail = new VerifyEmail()
            {
                Url = sendVerifyEmailMailDto.Url
            };
            string? template = await LoadMailTemplateAsync(VERIFY_EMAIL_TEMPLATE, verifyEmail, cancelationToken);
            if (!string.IsNullOrEmpty(template) && !string.IsNullOrEmpty(sendVerifyEmailMailDto.To))
            {
                SendMailDto sendMailDto = new SendMailDto()
                {
                    Body = template,
                    To = new string[] { sendVerifyEmailMailDto.To },
                    Subject = sendVerifyEmailMailDto.Subject
                };
                return await SendMailAsync(sendMailDto);
            }
            return false;
        }
    }
}
