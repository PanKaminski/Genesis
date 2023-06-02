using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Genesis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public string CurrentUserId =>
            this.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
    }
}
