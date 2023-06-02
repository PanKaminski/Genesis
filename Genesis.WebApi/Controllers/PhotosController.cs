using Genesis.App.Contract.Common.ApiModels;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Genesis.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService photoService;

        public PhotosController(IPhotoService photoService)
        {
            this.photoService = photoService;
        }

        public async Task<ActionResult<IEnumerable<PictureResponse>>> UploadAsync()
        {
            var files = Request.Form.Files;

            if (!files.Any())
            {
                throw new ArgumentException("There are no files to upload", nameof(files));
            }

            return Ok(await photoService.UploadAsync(files));
        }
    }
}
