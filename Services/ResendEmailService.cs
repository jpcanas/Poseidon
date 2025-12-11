using Poseidon.Models.Entities;
using Poseidon.Services.Interfaces;
using Resend;

namespace Poseidon.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;

        public ResendEmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task SendEmail(User logUser, string token)
        {
            string resetPasswordLink = "";
            //send example email 
            try
            {
                var message = new EmailMessage();
                message.From = "Poseidon <onboarding@resend.dev>";
                message.To.Add(logUser.Email);
                message.Subject = "Password Reset Request";
                message.HtmlBody = $"<h4>Hello {logUser.UserName} !</h4>" +
                    $"<p>We received a request to reset your password.</p></br>  " +
                    $"<p>Use the link below to choose a new password:</p></br>" +
                     $"<p>Token:{token}</p></br>" +
                    $"<a href=\"{resetPasswordLink}\" class=\"button\">Reset Password</a></br>" +
                    $"<p>If you didn’t request this, please ignore this message.</p></br>" +
                    $"<p>This link will expire in 10 minutes for security reasons.</p></br>";

                var res = await _resend.EmailSendAsync(message);
            }
            catch (Exception ex)
            {
                var exceptionEmail = ex;
            }
            
        }
    }
}
