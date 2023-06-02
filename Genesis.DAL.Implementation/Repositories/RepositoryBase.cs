using Genesis.Common.Exceptions;
using Genesis.Common.Extensions;
using Genesis.DAL.Implementation.Context;

namespace Genesis.DAL.Implementation.Repositories
{
    public abstract class RepositoryBase<T>
    {
        private readonly GenesisDbContext dbContext;

        protected GenesisDbContext DbContext => dbContext;

        protected RepositoryBase(GenesisDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected T Get(IQueryable<T> preparedModel, Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(preparedModel);

            return preparedModel.FirstOrDefault(item => predicate(item));
        }

        protected T GetSingle(IQueryable<T> preparedModel, Predicate<T> predicate, string errorMessage = null)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(preparedModel);

            if (preparedModel.TryGetSingleValue(item => predicate(item), out var account))
            {
                return account;
            }

            throw new GenesisApplicationException(errorMessage ?? "Impossible to get entity");
        }
    }
}
