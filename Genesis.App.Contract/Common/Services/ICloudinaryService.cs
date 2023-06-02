using Genesis.App.Contract.Models;
using Microsoft.AspNetCore.Http;

namespace Genesis.App.Contract.Common.Services
{
    public interface ICloudinaryService
    {
        Task<Picture> UploadImage(IFormFile picture);
    }
}
