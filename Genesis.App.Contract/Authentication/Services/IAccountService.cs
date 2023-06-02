using Genesis.App.Contract.Authentication.ApiModels;
using Genesis.App.Contract.Models.Authentication;
using Genesis.Common;

namespace Genesis.App.Contract.Authentication.Services;

public interface IAccountService
{
    AuthenticateResponse Login(string email, string password);
    
    AuthenticateResponse RefreshToken(string rtToken);

    void RevokeToken(string token);

    Task RegisterAsync(RegisterRequest viewModel, string origin);

    void VerifyEmail(string token);

    Task ForgotPasswordAsync(string email, string origin);

    void ResetPassword(string token, string password);

    void ValidateResetToken(string token);

    Task<PagedModel<Account>> GetConnections(int accountId, int page, int pageSize);
    IEnumerable<Account> GetConnections(int accountId);

    Task<PagedModel<Account>> GetInvites(int accountId, int page, int pageSize);

    Task<PagedModel<Account>> GetPendings(int accountId, int page, int pageSize);

    Task<PagedModel<Account>> SearchUsers(int accountId, int page, int pageSize);
}