using Poseidon.Enums;
using Poseidon.Models.Entities;
using Poseidon.Models.ViewModels;
using Poseidon.Repositories.Interfaces;
using Poseidon.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace Poseidon.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _basePath;
        private readonly IFileRecordRepository _fileRecordRepository;
        private static readonly string[] ImageContentTypes =
       ["image/jpeg", "image/png", "image/webp", "image/gif"];
        private static readonly int[] DocTypeUICropped = [2];
        public LocalStorageService(
            IWebHostEnvironment env,
            IConfiguration config,
            IFileRecordRepository fileRecordRepository)
        {
            _basePath = Path.Combine(env.WebRootPath, config["Storage:LocalPath"] ?? "private-uploads");
            _fileRecordRepository = fileRecordRepository;
            Directory.CreateDirectory(_basePath);
        }

        public async Task<FileRecordVM> SaveFileAsync(
            IFormFile file,
            ModuleType moduleType,
            DocumentType docType,
            int refId,
            string uploadedBy)
        {
            var folderPath = GenerateFilePath(moduleType, docType, refId, false);
            Directory.CreateDirectory(folderPath);

            var fileName = GenerateFileName(file);
            var fileKey = GenerateFileKey(moduleType, docType, refId, fileName, false);
            var fullPath = Path.Combine(_basePath, fileKey);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save thumbnail if it's an image
            string? thumbnailFileKey = null;
            if (ImageContentTypes.Contains(file.ContentType))
            {
                var thumbnailFolderPath = GenerateFilePath(moduleType, docType, refId, true);
                Directory.CreateDirectory(thumbnailFolderPath);
                var thumbnailFileName = Path.ChangeExtension(fileName, ".webp"); // Use .webp for thumbnails
                thumbnailFileKey = GenerateFileKey(moduleType, docType, refId, thumbnailFileName, true); 
                var thumbnailFullPath = Path.Combine(_basePath, thumbnailFileKey);

                using var image = await Image.LoadAsync(file.OpenReadStream());
                using var thumbnailStream = new FileStream(thumbnailFullPath, FileMode.Create);
                var resizeMode = DocTypeUICropped.Contains((int)docType) ? ResizeMode.Max : ResizeMode.Crop;
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = resizeMode,
                    Size = new Size(300, 300)
                }));

                await image.SaveAsync(thumbnailStream, WebpFormat.Instance);
            }

            var newFileRecord = new FileRecord
            {
                FileKey = fileKey,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                ModuleId = (int)moduleType,
                ReferenceId = refId,
                ModuleDocumentTypeId = (int)docType,
                ThumbnailKey = thumbnailFileKey,
                UploadedBy = uploadedBy,
            };

            FileRecord fr = await _fileRecordRepository.AddFileRecordAsync(newFileRecord);

            var fileRecordResult = new FileRecordVM
            {
                Id = fr.Id,
                FileKey = fileKey,
                ThumbnailKey = thumbnailFileKey,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                ModuleId = (int)moduleType,
                ReferenceId = refId,
                ModuleDocumentTypeId = (int)docType
            };
            return fileRecordResult;
        }
        public Task<Stream?> GetFileAsync(string fileKey)
        {
            var fullPath = Path.Combine(_basePath, fileKey);

            if (!File.Exists(fullPath))
                Task.FromResult<Stream?>(null);

            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Task.FromResult<Stream?>(stream);
        }
        public Task DeleteFileAsync(string fileKey)
        {
            throw new NotImplementedException();
        }
        private string GenerateFilePath(ModuleType moduleType, DocumentType docType, int refId, bool isThumbnail)
        {
            return isThumbnail
                ? Path.Combine(_basePath, moduleType.ToString(), docType.ToString(), refId.ToString(), "thumbnails")
                : Path.Combine(_basePath, moduleType.ToString(), docType.ToString(), refId.ToString());
        }
        private string GenerateFileKey(ModuleType moduleType, DocumentType docType, int refId, string fileName, bool isThumbnail)
        {
            return isThumbnail
              ? $"{moduleType.ToString()}/{docType.ToString()}/{refId.ToString()}/thumbnails/{fileName}"
            : $"{moduleType.ToString()}/{docType.ToString()}/{refId.ToString()}/{fileName}";
        }
        private string GenerateFileName(IFormFile file)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return $"{timestamp}_{shortGuid}{extension}";
        }
    }
}
