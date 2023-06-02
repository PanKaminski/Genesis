using Microsoft.AspNetCore.Http;

namespace Genesis.App.Contract.Common.ApiModels
{
    public class NewPictureRequest : EditPictureRequest
    {
        public IFormFile File { get; set; }
    }
}
