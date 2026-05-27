using Poseidon.Enums;
using Poseidon.Models.ViewModels;

namespace Poseidon.Services.Interfaces
{
    public interface IStorageService
    {
        Task<FileRecordVM> SaveFileAsync(IFormFile file, ModuleType moduleType, DocumentType docType, int refId, string uploadedBy);
        Task<Stream?> GetFileAsync(string fileKey);
        Task DeleteFileAsync(string fileKey);
       
    }
}
