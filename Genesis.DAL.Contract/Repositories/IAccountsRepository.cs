using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos.Account;
using Genesis.DAL.Contract.LoadOptions.Account;

namespace Genesis.DAL.Contract.Repositories;

public interface IAccountsRepository
{
    bool TryGetSingle(Predicate<AccountDto> predicate, out AccountDto accountDto, IList<AccountLoadOptions> loadOptions = null);

    IList<AccountDto> Get(IList<AccountLoadOptions> loadOptions = null);

    IEnumerable<AccountDto> Get(IEnumerable<int> ids, bool trackEntities, IList<AccountLoadOptions> loadOptions = null);

    AccountDto GetByEmail(string email, IList<AccountLoadOptions> loadOptions = null);

    AccountDto GetById(int id, IList<AccountLoadOptions> loadOptions = null);

    AccountDto GetByVerificationToken(string token, IList<AccountLoadOptions> loadOptions = null);

    AccountDto GetByActiveResetToken(string token, IList<AccountLoadOptions> loadOptions = null);

    AccountDto GetByRefreshToken(string token, IList<AccountLoadOptions> loadOptions = null);

    Task<PagedModel<AccountDto>> GetConnections(int accountId, ConnectionStatus status, bool checkFrom, bool checkTo, int page, int pageLimit, 
        bool trackEntities, IList<AccountLoadOptions> loadOptions = null);

    IEnumerable<AccountDto> GetConnections(int accountId, ConnectionStatus status, bool checkFrom, bool checkTo,
        bool trackEntities, IList<AccountLoadOptions> loadOptions = null);

    Task<PagedModel<AccountDto>> SearchUsers(int accountId, int page, int pageLimit,
        bool trackEntities, IList<AccountLoadOptions> loadOptions = null);

    int Create(AccountDto account);

    void RemoveOldRefreshTokens(AccountDto account, int rtTTLDays);

    void Update(AccountDto updatedAccount);

    bool Exists(string email);
}