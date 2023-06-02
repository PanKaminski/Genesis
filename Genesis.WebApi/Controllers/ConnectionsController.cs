using Genesis.App.Contract.Connections.ApiModels;
using Genesis.App.Contract.Models.Responses;
using Genesis.App.Implementation.Connections.Services;
using Genesis.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Genesis.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ConnectionsController : BaseApiController
    {
        private readonly ContactsService contactsService;

        public ConnectionsController(ContactsService contactsService)
        {
            this.contactsService = contactsService ?? throw new ArgumentNullException(nameof(contactsService));
        }

        [HttpGet]
        public async Task<ActionResult<PagedModel<ContactCardResponse>>> GetContactsAsync(int page, int pageSize) => 
            await contactsService.GetContacts(CurrentUserId, page, pageSize);

        [HttpGet]
        public async Task<ActionResult<PagedModel<ContactCardResponse>>> GetInvitesAsync(int page, int pageSize) =>
            await contactsService.GetInvites(CurrentUserId, page, pageSize);

        [HttpGet]
        public async Task<ActionResult<PagedModel<ContactCardResponse>>> GetPendingsAsync(int page, int pageSize) =>
            await contactsService.GetPendings(CurrentUserId, page, pageSize);

        [HttpGet]
        public async Task<ActionResult<PagedModel<UserCardResponse>>> SearchUsersAsync(int page, int pageSize) =>
            await contactsService.SearchUsers(CurrentUserId, page, pageSize);

        [HttpPost] 
        public ActionResult<ServerResponse<UpdateConnectionStatusResponse>> UpdateConnectionStatus(UsersConnectionStatusChnages model)
        {
            return contactsService.ChangeConectionStatus(model.UserId, model.IsRestrictionStatus, CurrentUserId);
        }
    }
}
