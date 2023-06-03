using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.Common.Extensions;
using Genesis.DAL.Contract.Dtos.Account;
using Genesis.DAL.Contract.LoadOptions.Account;
using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Implementation.Context;
using Genesis.DAL.Implementation.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Repositories
{
    public class AccountsRepository : RepositoryBase<AccountDto>, IAccountsRepository
    {
        public AccountsRepository(GenesisDbContext dbContext) : base(dbContext)
        {
        }

        public bool TryGetSingle(Predicate<AccountDto> predicate, out AccountDto accountDto, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);

            return accountsModel.TryGetSingleValue(predicate, out accountDto);
        }

        public IList<AccountDto> Get(IList<AccountLoadOptions> loadOptions = null) => PrepareModel(loadOptions).ToList();

        public AccountDto GetByEmail(string email, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);
            var account = GetSingle(accountsModel,
                acc => acc.Login == email,
                "Invalid email address");

            return account;
        }

        public AccountDto GetById(int id, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);
            var account = GetSingle(accountsModel,
                acc => acc.Id == id,
                "Invalid id");

            return account;
        }

        public AccountDto GetByVerificationToken(string token, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);
            var account = GetSingle(accountsModel,
                acc => acc.VerificationToken == token,
                "Invalid verification token");

            return account;
        }

        public AccountDto GetByActiveResetToken(string token, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);
            var account = GetSingle(accountsModel,
                a => a.ResetToken == token && a.ResetTokenExpires > DateTime.UtcNow,
                "Invalid reset token");

            return account;
        }

        public AccountDto GetByRefreshToken(string token, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);
            var account = GetSingle(accountsModel,
                acc => acc.RefreshTokens.TryGetValue(rt => rt.Token == token, out _),
                "Invalid refresh token");

            return account;
        }

        public int Create(AccountDto account)
        {
            var role = DbContext.Accounts.Any() ? Role.User : Role.Admin;

            account.Roles = new List<RoleDto> { DbContext.Roles.First(r => r.RoleName == role) };
            account.CreatedTime = DateTime.Now;

            DbContext.Accounts.Add(account);

            return account.Id;
        }

        public void RemoveOldRefreshTokens(AccountDto account, int rtTTLDays)
        {
            var tokensList = account.RefreshTokens.ToList();
            tokensList.RemoveAll(x => !x.IsActive && x.CreatedTime.AddDays(rtTTLDays) <= DateTime.UtcNow);

            account.RefreshTokens = tokensList;
        }

        public async Task<PagedModel<AccountDto>> GetConnections(int accountId, ConnectionStatus status, bool checkFrom, bool checkTo,
            int page, int pageLimit, bool trackEntities, IList<AccountLoadOptions> loadOptions = null)
        {
            return await GetConnections(accountId, status, checkFrom, checkTo, trackEntities, loadOptions)
                .AsQueryable().PaginateAsync(page, pageLimit);
        }

        public async Task<PagedModel<AccountDto>> SearchUsers(int accountId, int page, int pageLimit, bool trackEntities, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);

            if (!trackEntities) accountsModel = accountsModel.AsNoTracking();

            return await accountsModel.Where(p => 
                    p.OutgoingConnections.All(c => c.AccountToId != accountId) && 
                    p.IncomingConnections.All(c => c.AccountFromId != accountId) &&
                    p.Id != accountId)
                .PaginateAsync(page, pageLimit);
        }

        public IEnumerable<AccountDto> GetConnections(int accountId, ConnectionStatus status, bool checkFrom, bool checkTo, bool trackEntities, IList<AccountLoadOptions> loadOptions = null)
        {
            var accountsModel = PrepareModel(loadOptions);

            if (!trackEntities) accountsModel = accountsModel.AsNoTracking();

            return accountsModel.Where(p => (checkTo && p.IncomingConnections.Any(c => c.AccountFromId == accountId && c.Status == status)
                || checkFrom && p.OutgoingConnections.Any(c => c.AccountToId == accountId && c.Status == status))
                && accountId != p.Id);
        }

        public void Update(AccountDto updatedAccount) => DbContext.Update(updatedAccount);

        public bool Exists(string email) => DbContext.Accounts.TryGetSingleValue(a => a.Login == email, out _);

        public IEnumerable<AccountDto> Get(IEnumerable<int> ids, bool trackEntities, IList<AccountLoadOptions> loadOptions = null)
        {
            var model = PrepareModel(loadOptions);

            if (!trackEntities) model = model.AsNoTracking();

            return model.Where(acc => ids.Contains(acc.Id));
        }

        private IQueryable<AccountDto> PrepareModel(IList<AccountLoadOptions> loadOptions)
        {
            IQueryable<AccountDto> model = DbContext.Accounts.Include(acc => acc.RefreshTokens).Include(acc => acc.Roles);

            if (loadOptions is null) return model;

            if (loadOptions.Any(lo => lo == AccountLoadOptions.WithPersonData))
            {
                model = model.Include(acc => acc.RootPerson);
            }
            else if (loadOptions.Any(lo => lo == AccountLoadOptions.WithFullPersonData))
            {
                model = model.Include(acc => acc.RootPerson).ThenInclude(p => p.Biography);
                model = model.Include(acc => acc.RootPerson).ThenInclude(p => p.Photos);
            }

            if (loadOptions.Any(lo => lo == AccountLoadOptions.WithAvailableTrees))
            {
                model = model.Include(acc => acc.RootPerson).Include(a => a.AvailableTrees)
                    .ThenInclude(t => t.Persons);
            }

            if (loadOptions.Any(lo => lo == AccountLoadOptions.WithPersonalTrees))
            {
                model = model.Include(acc => acc.RootPerson).Include(a => a.PersonalTrees)
                    .ThenInclude(t => t.Persons);
            }

            if (loadOptions.Any(lo => lo == AccountLoadOptions.WithConnections))
            {
                model = model.Include(acc => acc.IncomingConnections).Include(acc => acc.OutgoingConnections);
            }

            return model;
        }
    }
}
