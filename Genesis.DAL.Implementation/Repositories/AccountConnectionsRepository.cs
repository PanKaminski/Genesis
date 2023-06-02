using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.Common.Exceptions;
using Genesis.DAL.Contract.Dtos.Account;
using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Implementation.Context;
using Genesis.DAL.Implementation.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Repositories
{
    public class AccountConnectionsRepository : RepositoryBase<AccountConnectionDto>, IAccountConnectionsRepository
    {
        public AccountConnectionsRepository(GenesisDbContext dbContext) : base(dbContext) { }

        public void AddConnection(AccountConnectionDto connection)
        {
            connection.AccountToId = connection.AccountToId == 0 ? connection.AccountTo.Id : connection.AccountToId;
            connection.AccountFromId = connection.AccountFromId == 0 ? connection.AccountFrom.Id : connection.AccountFromId;
            connection.AccountFrom = null;
            connection.AccountTo = null;
            connection.CreatedTime = DateTime.Now;

            DbContext.AccountConnections.Add(connection);
        }

        public IEnumerable<AccountConnectionDto> GetConnections(int userFromId, int userToId, bool trackEntities)
        {
            IQueryable<AccountConnectionDto> model = DbContext.AccountConnections;

            if (!trackEntities) model = model.AsNoTracking();

            return model.Where(ac => (ac.AccountFromId == userFromId || ac.AccountFromId == userToId)
                && (ac.AccountToId == userFromId || ac.AccountToId == userToId));
        }

        public async Task<PagedModel<AccountConnectionDto>> GetUserConnections(int accountId, int page, bool trackEntities, int limit = 40)
        {
            if (accountId < 1)
                throw new ArgumentException("Invalid account identifier", nameof(accountId));

            if (page < 1)
                throw new ArgumentException("Invalid page number", nameof(page));

            IQueryable<AccountConnectionDto> connectionsModel = DbContext.AccountConnections.Include(c => c.AccountFrom)
                .ThenInclude(a => a.RootPerson).ThenInclude(p => p.Photos);

            connectionsModel = connectionsModel.Include(c => c.AccountFrom)
                .ThenInclude(a => a.RootPerson).ThenInclude(p => p.Biography).ThenInclude(b => b.BirthPlace);

            connectionsModel = connectionsModel.Include(c => c.AccountFrom).ThenInclude(a => a.IncomingConnections);

            connectionsModel = connectionsModel.Include(c => c.AccountFrom).ThenInclude(a => a.OutgoingConnections);

            connectionsModel = connectionsModel.Include(c => c.AccountTo)
                .ThenInclude(a => a.RootPerson).ThenInclude(p => p.Photos);

            connectionsModel = connectionsModel.Include(c => c.AccountTo)
                .ThenInclude(a => a.RootPerson).ThenInclude(p => p.Biography).ThenInclude(b => b.BirthPlace);

            connectionsModel = connectionsModel.Include(c => c.AccountTo).ThenInclude(a => a.IncomingConnections);

            connectionsModel = connectionsModel.Include(c => c.AccountTo).ThenInclude(a => a.OutgoingConnections);

            if (!trackEntities) connectionsModel = connectionsModel.AsNoTracking();

            return await connectionsModel.Where(c => c.Status == ConnectionStatus.Accepted && (c.AccountFromId == accountId || c.AccountToId == accountId))
                .PaginateAsync(page, limit);
        }

        public void RemoveConnection(int connectionId)
        {
            var connection = DbContext.AccountConnections.FirstOrDefault(c => c.Id == connectionId);

            if (connection is null)
                throw new GenesisApplicationException("Contact is not found");

            DbContext.AccountConnections.Remove(connection);
        }

        public void UpdateConnectionStatus(int connectionId, ConnectionStatus status)
        {
            var connection = DbContext.AccountConnections.FirstOrDefault(c => c.Id == connectionId);

            if (connection is null)
                throw new GenesisApplicationException("Contact is not found");

            connection.Status = status;
        }
    }
}
