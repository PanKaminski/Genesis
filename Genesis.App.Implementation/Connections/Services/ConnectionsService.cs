using AutoMapper;
using Genesis.App.Contract.Connections.Services;
using Genesis.App.Contract.Models.Authentication;
using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos.Account;
using Genesis.DAL.Contract.UOW;

namespace Genesis.App.Implementation.Connections.Services
{
    public class ConnectionsService : IConnectionsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ConnectionsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedModel<AccountConnection>> GetConnectionsAsync(int accountId, int page, int pageLimit = 40)
        {
            var dtoModel = await unitOfWork.AccountConnectionsRepository.GetUserConnections(accountId, page, false, pageLimit);

            return new PagedModel<AccountConnection>
            {
                CurrentPage = dtoModel.CurrentPage,
                PageSize = dtoModel.CurrentPage,
                TotalCount = dtoModel.TotalCount,
                Items = mapper.Map<IEnumerable<AccountConnection>>(dtoModel.Items),
            };
        }

        public void RemoveConnection(int connectionId, bool saveChanges)
        {
            unitOfWork.AccountConnectionsRepository.RemoveConnection(connectionId);

            if (saveChanges) unitOfWork.Commit();
        }

        public int CreateNewConnection(int initiatorId, int userToId, ConnectionStatus status, bool saveChanges)
        {
            var connection = new AccountConnectionDto
            {
                AccountFromId = initiatorId,
                AccountToId = userToId,
                Status = status,
            };

            unitOfWork.AccountConnectionsRepository.AddConnection(connection);

            if (saveChanges) unitOfWork.Commit();

            return connection.Id;
        }

        public void UpdateConnectionStatus(int connectionId, ConnectionStatus status, bool saveChanges)
        {
            unitOfWork.AccountConnectionsRepository.UpdateConnectionStatus(connectionId, status);

            if (saveChanges) unitOfWork.Commit();
        }

        public IEnumerable<AccountConnection> GetConnections(int userFromId, int userToId)
        {
            var connections = unitOfWork.AccountConnectionsRepository.GetConnections(userFromId, userToId, false);

            return mapper.Map<IEnumerable<AccountConnection>>(connections);
        }
    }
}
