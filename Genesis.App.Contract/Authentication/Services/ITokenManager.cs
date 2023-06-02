using Genesis.App.Contract.Models.Authentication;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Authentication.Services;

public interface ITokenManager
{
    string GenerateJwt(int accountId,  IEnumerable<Role> roles);
    bool TryValidateToken(string token, out int accountId);
    RefreshToken GenerateRefreshToken();
}