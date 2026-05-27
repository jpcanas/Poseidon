using Poseidon.Models.Entities;

namespace Poseidon.Repositories.Interfaces
{
    public interface IFileRecordRepository
    {
        Task<FileRecord> AddFileRecordAsync(FileRecord record);
        Task<FileRecord?> GetFileRecordByIdAsync(int id);
    }
}