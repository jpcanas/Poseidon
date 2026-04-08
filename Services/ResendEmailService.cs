using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Services.Interfaces;
using Resend;
using Serilog;

namespace Poseidon.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly Dictionary<string, EmailTemplateConfig> _emailTemplates;

        public ResendEmailService(IResend resend, IConfiguration configuration)
        {
            _resend = resend;

            var templateList  = configuration
                .GetSection("EmailTemplates")
                .Get<List<EmailTemplateConfig>>() ?? new List<EmailTemplateConfig>();

            _emailTemplates = templateList
                .Where(t => t.IsActive)
                .ToDictionary(t => t.Name, t => t);
        }

        public async Task<bool> SendEmailAsync(string templateName, string toEmail, Dictionary<string, object> variables)
        {
            return await SendEmailAsync(templateName, new[] { toEmail }, variables);
        }

        public async Task<bool> SendEmailAsync(string templateName, string[] toEmails, Dictionary<string, object> variables)
        {
            try
            {
                if (!_emailTemplates.TryGetValue(templateName, out var template))
                {
                    Log.Error("Email template {TemplateName} was not found. Recipients: {Recipients}", 
                        templateName, toEmails);
                    return false; 
                }

                var resp = await _resend.EmailSendAsync(
                    new EmailMessage()
                    {
                        From = $"{template.FromName} <{template.FromEmail}>",
                        To = toEmails,
                        Subject = template.Subject,
                        Template = new EmailMessageTemplate()
                        {
                            TemplateId = template.TemplateId,
                            Variables = variables,
                        }
                    }
                );

                var success = resp != null;

                if (success)
                {
                    Log.Information("Email sent successfully. Template: {TemplateName}, Recipients: {Recipients}",
                        templateName, toEmails);
                }else
                {
                    Log.Warning("Email send returned null response. Template: {TemplateName}, Recipients: {Recipients}",
                       templateName, toEmails);
                }
                   
                return success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Email sending failed. Template: {TemplateName}, Recipients: {Recipients}",
                     templateName, toEmails);
                return false;
            }
        }
    }
}
