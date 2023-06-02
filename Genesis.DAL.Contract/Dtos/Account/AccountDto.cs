using Genesis.Common;

namespace Genesis.DAL.Contract.Dtos.Account;

public class AccountDto : AuditableEntity
{
    public string Login { get; set; }

    public IList<RoleDto> Roles { get; set; }
    
    public string PasswordHash { get; set; }
    
    public bool AcceptTerms { get; set; }
    
    public string VerificationToken { get; set; }
    
    public DateTime? Verified { get; set; }
    
    public bool IsVerified => Verified.HasValue || PasswordResetTime.HasValue;

    public string ResetToken { get; set; }
    
    public DateTime? ResetTokenExpires { get; set; }
    
    public DateTime? PasswordResetTime { get; set; }

    public string CountryCode { get; set; }

    public int? CityId { get; set; }
    
    public IList<RefreshTokenDto> RefreshTokens { get; set; }

    public IList<GenealogicalTreeDto> AvailableTrees { get; set; }

    public IList<GenealogicalTreeDto> PersonalTrees { get; set; }

    public IList<AccountConnectionDto> OutgoingConnections { get; set; }

    public IList<AccountConnectionDto> IncomingConnections { get; set; }

    public PersonDto RootPerson { get; set; }
}