using Poseidon.Data;
using Poseidon.Models.Entities;
using Poseidon.Repositories.Interfaces;

namespace Poseidon.Repositories
{
    public class FileRecordRepository : IFileRecordRepository
    {
        private readonly PoseidonDbContext _context;
        public FileRecordRepository(PoseidonDbContext context)
        {
            _context = context;
        }
        public async Task<FileRecord> AddFileRecordAsync(FileRecord record)
        {
            _context.FileRecords.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<FileRecord?> GetFileRecordByIdAsync(int id)
        {
            return await _context.FileRecords.FindAsync(id);
        }
    }
}
