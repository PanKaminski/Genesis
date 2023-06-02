using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Models.Forms;
using Genesis.App.Contract.Models.Responses;
using Genesis.App.Implementation.Dashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Genesis.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PersonsController : ControllerBase
    {
        private readonly DashboardToolService dashboardToolService;

        public PersonsController(DashboardToolService dashboardToolService)
        {
            this.dashboardToolService = dashboardToolService ?? throw new ArgumentNullException(nameof(dashboardToolService));
        }

        [HttpPost]
        public ActionResult<Form> GetForm(PersonFormParams formParams)
        {
            if (formParams is null) return BadRequest();

            return Ok(dashboardToolService.GetPersonForm(formParams));
        }

        [HttpPost]
        public async Task<ActionResult<ServerResponse<TreeNodeResponse>>> SaveFormAsync(PersonEditModel personEditModel)
        {
            if (personEditModel is null) 
                return BadRequest();

            return await dashboardToolService.SavePersonFormAsync(personEditModel);
        }

        [HttpDelete]
        public ActionResult<ServerResponse> DeletePerson(int personId)
        {
            if (personId < 1) return BadRequest();

            return dashboardToolService.DeletePerson(personId);
        }
    }
}
