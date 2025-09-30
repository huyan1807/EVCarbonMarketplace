using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Settings;
using EVCarbonMarketplace.Service.Interface;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class UploadService : IUploadService
    {
        private readonly Cloudinary _cloudinary;
        private readonly StorageClient _storageClient;
        private readonly string _firebaseBucket;

        public UploadService(Cloudinary cloudinary,
                             IConfiguration config)
        {
            _cloudinary = cloudinary;
     
          
            var credentialPath = config["Firebase:CredentialPath"];
            _firebaseBucket = config["Firebase:Bucket"];

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });
            }

            _storageClient = StorageClient.Create(GoogleCredential.FromFile(credentialPath));
        }


        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null)
            {
                throw new NotFoundException();
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParam = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, stream),
                    Folder = "image_avt",
                    PublicId = Guid.NewGuid().ToString(),
                    Transformation = new Transformation().Quality("auto:low")
                                                         .FetchFormat("webp")
                                                         .Width(1024)
                                                         .Crop("limit")
                };


                var uploadResult = await _cloudinary.UploadAsync(uploadParam);
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    throw new Exception("Không tải được hình ảnh lên Cloudinary.");
                }
            }
        }

       

   
        public async Task<string> UploadToFirebaseAsync(IFormFile fileToUpload)
        {
            if (fileToUpload == null) throw new NotFoundException();

            var allowedExtensions = new[] { ".docx", ".pdf", ".mov", ".xlsx", ".mp4", ".jpg", ".txt" };
            var ext = Path.GetExtension(fileToUpload.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Định dạng file không được hỗ trợ.");

            using var stream = fileToUpload.OpenReadStream();

            var fileName = $"certificates/{Guid.NewGuid()}-{fileToUpload.FileName}";

            await _storageClient.UploadObjectAsync(
                bucket: _firebaseBucket,
                objectName: fileName,
                contentType: fileToUpload.ContentType,
                source: stream
            );

            return $"https://firebasestorage.googleapis.com/v0/b/{_firebaseBucket}/o/{Uri.EscapeDataString(fileName)}?alt=media";
        }



    }
}
