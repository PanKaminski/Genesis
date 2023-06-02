using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos.Account;

namespace Genesis.DAL.Contract.Repositories
{
    public interface IAccountConnectionsRepository
    {
        Task<PagedModel<AccountConnectionDto>> GetUserConnections(int accountId, int page, bool trackEntities, int limit = 40);

        IEnumerable<AccountConnectionDto> GetConnections(int userFromId, int userToId, bool trackEntities);

        void UpdateConnectionStatus(int connectionId, ConnectionStatus status);

        void RemoveConnection(int connectionId);

        void AddConnection(AccountConnectionDto connection);
    }
}
