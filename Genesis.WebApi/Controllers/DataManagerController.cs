using Genesis.App.Contract.Models.Tables;
using Genesis.App.Implementation.DataManager;
using Microsoft.AspNetCore.Mvc;

namespace Genesis.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataManagerController : BaseApiController
    {
        private readonly DataManagerToolService dataManagerToolService;

        public DataManagerController(DataManagerToolService dataManagerToolService)
        {
            this.dataManagerToolService = dataManagerToolService;
        }

        [HttpGet]
        public Table GetPersonsTable() => dataManagerToolService.GetPersonsTable(CurrentUserId);
    }
}
