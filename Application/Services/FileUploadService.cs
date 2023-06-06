using Application.Interfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FileUploadService : IFileUpload
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void DeleteAll(string staffId)
        {
            var directory = "uploads\\";
            var uploadPath = Path.Combine(_env.WebRootPath, directory, staffId);
            if(Directory.Exists(uploadPath))
            {
                Directory.Delete(uploadPath, true);
            }
        }

        public void DeleteFile(StaffQualificationViewModel model)
        {
            var directory = "uploads\\";
            var uploadPath = Path.Combine(_env.WebRootPath, directory, model.StaffId.ToString());
            var filePath = Path.Combine(uploadPath, model.DownloadFileLink);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public async Task<string> Upload(StaffQualificationViewModel model)
        {
            var validate = ValidateUpload(model.FormFile);
            if (!validate)
                return string.Empty;
            var directory = "uploads\\";
            var uploadPath = Path.Combine(_env.WebRootPath, directory, model.StaffId.ToString());
            if(!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
            var filePath = Path.Combine(uploadPath, model.FormFile.FileName);
            using(var stream = File.Create(filePath))
            {
                await model.FormFile.CopyToAsync(stream);
            }
            return filePath;
        }

        private bool ValidateUpload(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            if (ext != ".pdf")
                return false;
            Dictionary<string, List<byte[]>> fileSignatures = new()
            {
                { ".pdf", new List<byte[]>
                    {
                        new byte[] { 0x25, 0x50, 0x44, 0x46 },
                    }
                },
            };
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var signatures = fileSignatures[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
