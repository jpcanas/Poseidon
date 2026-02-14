using Poseidon.Models.Entities;

namespace Poseidon.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string templateName, string toEmail, Dictionary<string, object> variables);
        Task<bool> SendEmailAsync(string templateName, string[] toEmails, Dictionary<string, object> variables);
    }
}
