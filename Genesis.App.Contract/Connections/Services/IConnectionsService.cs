using Genesis.App.Contract.Models.Authentication;
using Genesis.Common;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Connections.Services
{
    public interface IConnectionsService
    {
        Task<PagedModel<AccountConnection>> GetConnectionsAsync(int accountId, int page, int pageLimit = 40);

        IEnumerable<AccountConnection> GetConnections(int userFromId, int userToId);

        void RemoveConnection(int connectionId, bool saveChanges);

        int CreateNewConnection(int initiatorId, int userToId, ConnectionStatus status, bool saveChanges);

        void UpdateConnectionStatus(int connectionId, ConnectionStatus status, bool saveChanges);
    }
}
