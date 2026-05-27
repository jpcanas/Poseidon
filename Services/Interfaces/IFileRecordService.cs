using Poseidon.Models.Entities;

namespace Poseidon.Services.Interfaces
{
    public interface IFileRecordService
    {
        Task<FileRecord?> GetFileRecordById(int id);
    }
}