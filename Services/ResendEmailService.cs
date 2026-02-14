using Poseidon.Configurations;
using Poseidon.Models.Entities;
using Poseidon.Services.Interfaces;
using Resend;

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
                    // Log exception properly
                    return false; // Template not found
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

                return resp != null;
            }
            catch (Exception ex)
            {
                // Log exception properly
                return false;

            }
        }
    }
}
