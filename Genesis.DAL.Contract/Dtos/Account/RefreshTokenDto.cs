using Genesis.Common;

namespace Genesis.DAL.Contract.Dtos.Account;

public class RefreshTokenDto : AuditableEntity
{
    public string Token { get; set; }
    
    public DateTime Expires { get; set; }
    
    public DateTime? Revoked { get; set; }
    
    public string ReplacedByToken { get; set; }
    
    public string ReasonRevoked { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= Expires;
    
    public bool IsActive => Revoked is null && !IsExpired;

    public AccountDto Account { get; set; }
}