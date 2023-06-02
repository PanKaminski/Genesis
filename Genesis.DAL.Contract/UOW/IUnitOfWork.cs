using Genesis.DAL.Contract.Repositories;

namespace Genesis.DAL.Contract.UOW
{
    public interface IUnitOfWork
    {
        IAccountsRepository AccountsRepository { get; }
        IGenealogicalTreesRepository GenealogicalTreesRepository { get; }
        IPersonsRepository PersonsRepository { get; }
        IPicturesRepository PicturesRepository { get; }
        IRelationsRepository RelationsRepository { get; }
        IAccountConnectionsRepository AccountConnectionsRepository { get; }

        void Commit();
        Task CommitAsync();
        void RevertChanges();
    }
}
