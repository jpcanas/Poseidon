using Poseidon.Models.Entities;
using Poseidon.Repositories.Interfaces;
using Poseidon.Services.Interfaces;

namespace Poseidon.Services
{
    public class FileRecordService : IFileRecordService
    {
        private readonly IFileRecordRepository _fileRecordRepository;
        public FileRecordService(IFileRecordRepository fileRecordRepository)
        {
            _fileRecordRepository = fileRecordRepository;
        }
        public async Task<FileRecord?> GetFileRecordById(int id)
        {
            return await _fileRecordRepository.GetFileRecordByIdAsync(id);
        }
    }
}
