using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Dashboard.Services;
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
    public class FamilyTreesController : BaseApiController
    {
        private readonly DashboardToolService dashboardToolService;
        private readonly IPersonService personService;

        public FamilyTreesController(IPersonService personService, DashboardToolService dashboardToolService)
        {
            this.personService = personService ?? throw new ArgumentNullException(nameof(personService));
            this.dashboardToolService = dashboardToolService ?? throw new ArgumentNullException(nameof(dashboardToolService));
        }

        [HttpPost]
        public async Task<ActionResult<ServerResponse<GenealogicalTreeItemResponse>>> SaveTreeFormAsync(TreeEditorInfo data) =>
            await dashboardToolService.SaveTreeFormAsync(data, CurrentUserId);

        [HttpGet]
        public ActionResult<Form> GetTreeForm(int treeId) => Ok(dashboardToolService.GetTreeForm(treeId, CurrentUserId));

        [HttpGet]
        public ActionResult<TreesListResponse> GetAvailableTrees() => dashboardToolService.GetAllUserTrees(CurrentUserId);

        [HttpGet]
        public ActionResult<bool> CanLoadTree(int treeId) => dashboardToolService.CanAccessTree(treeId, CurrentUserId);

        [HttpGet]
        public IAsyncEnumerable<TreeNodeResponse> GetTreePersonsAsync(int treeId) => personService.GetTreePersonsAsync(treeId, CurrentUserId);
    }
}
