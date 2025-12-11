using Poseidon.Models.Entities;

namespace Poseidon.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(User logUser, string token);
    }
}
