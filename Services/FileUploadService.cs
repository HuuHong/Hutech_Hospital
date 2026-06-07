using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HUTECH_Hospital.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> UploadFileAsync(IFormFile? file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            // Check file extension
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExts = { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExts.Contains(ext))
                throw new InvalidOperationException("Chỉ chấp nhận các tệp ảnh định dạng .jpg, .jpeg, .png, .webp");

            // Define physical path: wwwroot/uploads/[folderName]
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Create unique file name
            string uniqueFileName = Guid.NewGuid().ToString() + ext;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return URL string
            return $"/uploads/{folderName}/{uniqueFileName}";
        }

        public bool DeleteFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            // Assuming filePath starts with "/uploads/..."
            // Strip the leading '/'
            string relativePath = filePath.TrimStart('/');
            string absolutePath = Path.Combine(_env.WebRootPath, relativePath);

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
                return true;
            }

            return false;
        }
    }
}
