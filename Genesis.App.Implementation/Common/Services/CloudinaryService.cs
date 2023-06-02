using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Implementation.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Genesis.App.Implementation.Common.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;
        private readonly CloudinarySettings settings;

        public CloudinaryService(IOptions<CloudinarySettings> settings)
        {
            this.settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            var clodinaryAccount = new Account(this.settings.CloudName, this.settings.ApiKey, this.settings.SecretKey);
            cloudinary = new Cloudinary(clodinaryAccount);
        }


        public async Task<Picture> UploadImage(IFormFile picture)
        {
            var uploadResult = new ImageUploadResult();

            if (picture.Length > 0)
            {
                using var stream = picture.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(picture.Name, stream),
                };

                uploadResult = await cloudinary.UploadAsync(uploadParams);
            }

            return new Picture(uploadResult.Uri.ToString(), uploadResult.PublicId);        
        }
    }
}
