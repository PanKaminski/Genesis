using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Connections.ApiModels;
using Genesis.App.Contract.Connections.Services;
using Genesis.App.Contract.Models.Authentication;
using Genesis.App.Contract.Models.Forms;
using Genesis.App.Contract.Models.Responses;
using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.Common.Exceptions;
using Genesis.Common.Extensions;

namespace Genesis.App.Implementation.Connections.Services
{
    public class ContactsService
    {
        private readonly IAccountService accountService;
        private readonly IConnectionsService connectionsService;

        public ContactsService(IAccountService accountService, IConnectionsService connectionsService)
        {
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            this.connectionsService = connectionsService ?? throw new ArgumentNullException(nameof(connectionsService));
        }

        public async Task<PagedModel<ContactCardResponse>> GetContacts(string currentAccountId, int page,int pageSize)
        {
            if (!int.TryParse(currentAccountId, out int accountId) || accountId < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentAccountId));
            }

            var accounts = await accountService.GetConnections(accountId, page, pageSize);

            return BuildConnections(accounts, accountId);
        }

        public async Task<PagedModel<ContactCardResponse>> GetInvites(string currentAccountId, int page, int pageSize)
        {
            if (!int.TryParse(currentAccountId, out int accountId) || accountId < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentAccountId));
            }

            var accounts = await accountService.GetInvites(accountId, page, pageSize);

            return BuildConnections(accounts, accountId);
        }

        public async Task<PagedModel<ContactCardResponse>> GetPendings(string currentAccountId, int page, int pageSize)
        {
            if (!int.TryParse(currentAccountId, out int accountId) || accountId < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentAccountId));
            }

            var accounts = await accountService.GetPendings(accountId, page, pageSize);

            return BuildConnections(accounts, accountId);
        }


        public async Task<PagedModel<UserCardResponse>> SearchUsers(string currentAccountId, int page, int pageSize)
        {
            if (!int.TryParse(currentAccountId, out int accountId) || accountId < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentAccountId));
            }

            var accounts = await accountService.SearchUsers(accountId, page, pageSize);

            var users = new List<UserCardResponse>();

            foreach (var account in accounts.Items)
            {
                var userCard = new UserCardResponse
                {
                    UserId = account.Id,
                    UserName = account.GetRootPerson().GetTreeNodeName(),
                    ConnectionsCount = account.IncomingConnections.Count + account.OutgoingConnections.Count,
                    Avatar = account.GetRootPerson().Photos.FirstOrDefault(p => p.IsMain)?.Url ?? null,
                    Country = account.GetRootPerson().Biography.BirthPlace?.Country,
                    City = account.GetRootPerson().Biography.BirthPlace?.Settlement,
                    Buttons = GetCardButtons(null, null, accountId),
                };

                users.Add(userCard);
            }

            return new PagedModel<UserCardResponse>
            {
                PageSize = accounts.PageSize,
                TotalCount = accounts.TotalCount,
                CurrentPage = accounts.CurrentPage,
                Items = users,
            };
        }

        public ServerResponse<UpdateConnectionStatusResponse> ChangeConectionStatus(int userToId, bool isRestrictionStatus, string currentAccountId)
        {
            if (!int.TryParse(currentAccountId, out int currentAccountIdResult) || currentAccountIdResult < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentAccountId));
            }

            var connections = connectionsService.GetConnections(currentAccountIdResult, userToId).ToList();

            var connection = connections.Count() > 1 ? connections.FirstOrDefault(c => c.AccountFromId == currentAccountIdResult) : connections.FirstOrDefault();
            userToId = connection?.AccountToId is null ? userToId : connection.AccountToId;

            return ResolveNextConnectionStatus(connection?.Id, connection?.Status, currentAccountIdResult, userToId, !isRestrictionStatus);
        }

        private ServerResponse<UpdateConnectionStatusResponse> ResolveNextConnectionStatus(int? connectionId, ConnectionStatus? currentStatus,
            int currentUserId, int userToId, bool isPositiveStatus)
        {
            try
            {
                var newStatus = GetNextConnectionStatus(currentStatus, currentUserId, userToId, isPositiveStatus);

                if (newStatus is null)
                {
                    connectionsService.RemoveConnection(connectionId.Value, true);
                }
                else if (newStatus is ConnectionStatus.Pending or ConnectionStatus.Blocked)
                {
                    connectionId = connectionsService.CreateNewConnection(currentUserId, userToId, newStatus.Value, true);
                }
                else
                {
                    connectionsService.UpdateConnectionStatus(connectionId.Value, newStatus.Value, true);
                }

                var updateResult = new UpdateConnectionStatusResponse
                {
                    UserId = userToId,
                    ConnectionId = connectionId,
                    Status = newStatus,
                    Buttons = GetCardButtons(newStatus, userToId, currentUserId),
                };

                return new ServerResponse<UpdateConnectionStatusResponse>(ResultCode.Done, updateResult);
            }
            catch (GenesisApplicationException exc)
            {
                return new ServerResponse<UpdateConnectionStatusResponse>(ResultCode.Failed, null, exc.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ConnectionStatus? GetNextConnectionStatus(ConnectionStatus? currentStatus,
            int currentUserId, int userToId, bool isPositiveStatus) => currentStatus switch
            {
                ConnectionStatus.Pending when isPositiveStatus && currentUserId == userToId => ConnectionStatus.Accepted,
                ConnectionStatus.Pending when !isPositiveStatus && currentUserId == userToId => ConnectionStatus.Declined,
                ConnectionStatus.Pending when !isPositiveStatus => null,
                ConnectionStatus.Accepted when !isPositiveStatus => null,
                ConnectionStatus.Blocked when isPositiveStatus && currentUserId != userToId => null,
                _ when isPositiveStatus => ConnectionStatus.Pending,
                _ when !isPositiveStatus => ConnectionStatus.Blocked,
                _ => throw new GenesisApplicationException("Attempt to set invalid connection status"),
            };

        private PagedModel<ContactCardResponse> BuildConnections(PagedModel<Account> accounts, int accountId)
        {
            var contacts = new List<ContactCardResponse>();

            foreach (var account in accounts.Items)
            {
                var connection = account.IncomingConnections.TryGetSingleValue(c => c.AccountFromId == accountId ||
                c.AccountToId == accountId, out var incConnection) ? incConnection : account.OutgoingConnections.First(c => c.AccountFromId == accountId ||
                c.AccountToId == accountId);

                var contactCard = new ContactCardResponse
                {
                    ConnectionId = connection.Id,
                    Status = connection.Status,
                    UserId = account.Id,
                    UserName = account.GetRootPerson().GetTreeNodeName(),
                    ConnectionsCount = account.IncomingConnections.Where(c => c.Status == ConnectionStatus.Accepted).Count()
                        + account.OutgoingConnections.Where(c => c.Status == ConnectionStatus.Accepted).Count(),
                    Avatar = account.GetRootPerson().Photos.FirstOrDefault(p => p.IsMain)?.Url ?? null,
                    Country = account.GetRootPerson().Biography.BirthPlace?.Country,
                    City = account.GetRootPerson().Biography.BirthPlace?.Settlement,
                    Buttons = GetCardButtons(connection.Status, connection.AccountToId, accountId),
                };

                contacts.Add(contactCard);
            }

            return new PagedModel<ContactCardResponse>
            {
                PageSize = accounts.PageSize,
                TotalCount = accounts.TotalCount,
                CurrentPage = accounts.CurrentPage,
                Items = contacts,
            };
        }

        private List<Button> GetCardButtons(ConnectionStatus? status, int? userTo, int currentUser)
        {
            return status switch
            {
                ConnectionStatus.Pending when userTo == currentUser => new List<Button>
                    {
                        new Button(ButtonType.CardRestrictionButton, "Decline"),
                        new Button(ButtonType.CardConnectionButton, "Accept"),
                    },
                ConnectionStatus.Pending => new List<Button>
                    {
                        new Button(ButtonType.CardRestrictionButton, "Retract"),
                        new Button(ButtonType.CardConnectionButton, "Waiting...", true),
                    },
                ConnectionStatus.Accepted => new List<Button>
                    {
                        new Button(ButtonType.CardRestrictionButton, "Remove"),
                        new Button(ButtonType.CardConnectionButton, "Message", true),
                    },
                ConnectionStatus.Blocked => new List<Button>
                    {
                        new Button(ButtonType.CardConnectionButton, "Unblock"),
                    },
                _ => new List<Button>
                    {
                        new Button(ButtonType.CardRestrictionButton, "Block"),
                        new Button(ButtonType.CardConnectionButton, "Connect"),
                    },
            };
        }
    }
}
