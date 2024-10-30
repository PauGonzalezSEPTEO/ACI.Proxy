using ACI.HAM.Mail.Dtos;
using ACI.HAM.Mail.Helpers;
using ACI.HAM.Mail.Models;
using ACI.HAM.Mail.Settings;
using ACI.HAM.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ACI.HAM.Mail.Services
{
    public interface IMailService
    {
        string[] GetModels();

        Task<GetTemplateByNameDto> GetTemplateByNameAsync(string name, string languageCode, CancellationToken cancelationToken = default);

        Task<bool> SendApiKeyRotationMailAsync(SendApiKeyRotationMailDto sendApiKeyRotationMailDto, CancellationToken cancelationToken = default);

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
        const string API_KEY_ROTATION_EMAIL_TEMPLATE = "ApiKeyRotation";
        const string CHANGE_EMAIL_TEMPLATE = "ChangeEmail";
        const string LOCKOUT_TEMPLATE = "Lockout";
        const string PASSWORD_CHANGE_TEMPLATE = "PasswordChange";
        const string PASSWORD_RESET_TEMPLATE = "PasswordReset";
        const string TWO_FACTOR_TEMPLATE = "TwoFactor";
        const string VERIFY_EMAIL_TEMPLATE = "VerifyEmail";

        private readonly IStringLocalizer<MailService> _localizer;
        private readonly MailSettings _mailSettings;
        private readonly MailTemplateHelper _mailTemplateHelper;
        private readonly UISettings _uiSettings;

        public MailService(IOptions<MailSettings> mailSettings, IStringLocalizer<MailService> localizer, IOptions<UISettings> uiSettings, MailTemplateHelper mailTemplateHelper)
        {
            _mailSettings = mailSettings.Value;
            _uiSettings = uiSettings.Value;
            _localizer = localizer;
            _mailTemplateHelper = mailTemplateHelper;
        }

        private async Task<GetTemplateByNameDto> CreateModelAndLoadTemplateByTemplateNameAsync<T>(string name, string languageCode, CancellationToken cancelationToken = default) where T : class
        {
            var model = name switch
            {
                API_KEY_ROTATION_EMAIL_TEMPLATE => new ApiKeyRotation() as T,
                CHANGE_EMAIL_TEMPLATE => new ChangeEmail() as T,
                LOCKOUT_TEMPLATE => new Lockout() as T,
                PASSWORD_CHANGE_TEMPLATE => Type.Missing as T,
                PASSWORD_RESET_TEMPLATE => new PasswordReset() as T,
                TWO_FACTOR_TEMPLATE => new TwoFactor() as T,
                VERIFY_EMAIL_TEMPLATE => new VerifyEmail() as T,
                _ => null,
            };
            List<string>? propertyNames = null;
            if (model != null)
            {
                propertyNames = model?.GetType().GetProperties().Select(p => p.Name).ToList() ?? new List<string>();
            }
            var template = await _mailTemplateHelper.LoadMailTemplateAsync(name, model, languageCode, cancelationToken);
            return new GetTemplateByNameDto
            {
                CustomFields = propertyNames,
                HTMLContent = template,
            };
        }

        public string[] GetModels()
        {
            return new string[]
            {
                    API_KEY_ROTATION_EMAIL_TEMPLATE,
                    CHANGE_EMAIL_TEMPLATE,
                    LOCKOUT_TEMPLATE,
                    PASSWORD_CHANGE_TEMPLATE,
                    PASSWORD_RESET_TEMPLATE,
                    TWO_FACTOR_TEMPLATE,
                    VERIFY_EMAIL_TEMPLATE
            };
        }

        public async Task<GetTemplateByNameDto> GetTemplateByNameAsync(string name, string languageCode, CancellationToken cancelationToken = default)
        {
            var result = name switch
            {
                API_KEY_ROTATION_EMAIL_TEMPLATE => await CreateModelAndLoadTemplateByTemplateNameAsync<ApiKeyRotation>(name, languageCode, cancelationToken),
                CHANGE_EMAIL_TEMPLATE => await CreateModelAndLoadTemplateByTemplateNameAsync<ChangeEmail>(name, languageCode, cancelationToken),
                LOCKOUT_TEMPLATE => await CreateModelAndLoadTemplateByTemplateNameAsync<Lockout>(name, languageCode, cancelationToken),
                PASSWORD_CHANGE_TEMPLATE => await CreateModelAndLoadTemplateByTemplateNameAsync<Type>(name, languageCode, cancelationToken),
                PASSWORD_RESET_TEMPLATE => await CreateModelAndLoadTemplateByTemplateNameAsync<PasswordReset>(name, languageCode, cancelationToken),
                TWO_FACTOR_TEMPLATE => await CreateModelAndLoadTemplateByTemplateNameAsync<TwoFactor>(name, languageCode, cancelationToken),
                VERIFY_EMAIL_TEMPLATE => await CreateModelAndLoadTemplateByTemplateNameAsync<VerifyEmail>(name, languageCode, cancelationToken),
                _ => null,
            };
            if (result == null)
            {
                throw new Exception(_localizer["Template not found"]);
            }
            return result;
        }

        public async Task<bool> SendApiKeyRotationMailAsync(SendApiKeyRotationMailDto sendApiKeyRotationMailDto, CancellationToken cancelationToken = default)
        {
            if ((sendApiKeyRotationMailDto != null) && (sendApiKeyRotationMailDto.To != null))
            {
                ApiKeyRotation apiKeyRotation = new ApiKeyRotation()
                {
                    Expiration = sendApiKeyRotationMailDto.Expiration,
                    Url = sendApiKeyRotationMailDto.Url
                };
                string? template = await _mailTemplateHelper.LoadMailTemplateAsync(API_KEY_ROTATION_EMAIL_TEMPLATE, apiKeyRotation, null, cancelationToken);
                if (!string.IsNullOrEmpty(template))
                {
                    SendMailDto sendMailDto = new SendMailDto()
                    {
                        Body = template,
                        To = new string[] { sendApiKeyRotationMailDto.To },
                        Subject = sendApiKeyRotationMailDto.Subject
                    };
                    return await SendMailAsync(sendMailDto);
                }
            }
            return false;
        }

        public async Task<bool> SendChangeEmailMailAsync(SendChangeEmailMailDto sendChangeEmailMailDto, CancellationToken cancelationToken = default)
        {
            if ((sendChangeEmailMailDto != null) && (sendChangeEmailMailDto.To != null))
            {
                ChangeEmail changeEmail = new ChangeEmail()
                {
                    Url = sendChangeEmailMailDto.Url
                };
                string? template = await _mailTemplateHelper.LoadMailTemplateAsync(CHANGE_EMAIL_TEMPLATE, changeEmail, null, cancelationToken);
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
                string? template = await _mailTemplateHelper.LoadMailTemplateAsync(LOCKOUT_TEMPLATE, lockout, null, cancelationToken);
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
                string? template = await _mailTemplateHelper.LoadMailTemplateAsync(PASSWORD_CHANGE_TEMPLATE, Type.Missing, null, cancelationToken);
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
            string? template = await _mailTemplateHelper.LoadMailTemplateAsync(PASSWORD_RESET_TEMPLATE, passwordReset, null, cancelationToken);
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
                string? template = await _mailTemplateHelper.LoadMailTemplateAsync(TWO_FACTOR_TEMPLATE, twoFactor, null, cancelationToken);
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
            string? template = await _mailTemplateHelper.LoadMailTemplateAsync(VERIFY_EMAIL_TEMPLATE, verifyEmail, null, cancelationToken);
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
