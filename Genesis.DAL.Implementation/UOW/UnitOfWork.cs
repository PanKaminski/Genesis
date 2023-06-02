using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Contract.UOW;
using Genesis.DAL.Implementation.Context;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.UOW
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly GenesisDbContext dbContext;

        public UnitOfWork(
            GenesisDbContext dbContext,
            IAccountsRepository accountsRepository,
            IGenealogicalTreesRepository genealogicalTreesRepository,
            IPersonsRepository personsRepository,
            IPicturesRepository picturesRepository,
            IRelationsRepository relationsRepository,
            IAccountConnectionsRepository connectionsRepository
        )
        {
            this.dbContext = dbContext;
            AccountsRepository = accountsRepository;
            GenealogicalTreesRepository = genealogicalTreesRepository;
            PersonsRepository = personsRepository;
            PicturesRepository = picturesRepository;
            RelationsRepository = relationsRepository;
            AccountConnectionsRepository = connectionsRepository;
        }

        public IAccountsRepository AccountsRepository { get; }
        public IGenealogicalTreesRepository GenealogicalTreesRepository { get; }
        public IPersonsRepository PersonsRepository { get; }
        public IPicturesRepository PicturesRepository { get; }
        public IRelationsRepository RelationsRepository { get; }
        public IAccountConnectionsRepository AccountConnectionsRepository { get; }

        public void Commit() => dbContext.SaveChanges();

        public async Task CommitAsync() => await dbContext.SaveChangesAsync();

        public void RevertChanges()
        {
            foreach (var entry in dbContext.ChangeTracker.Entries()
                  .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
