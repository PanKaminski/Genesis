using Genesis.Common;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Models.Authentication;

public class Account : AuditableEntity
{
    public Account()
    {

    }

    public Account(int id)
    {
        Id = id;
    }

    public string Login { get; set; }

    public IList<Role> Roles { get; set; }
    
    public string PasswordHash { get; set; }
    
    public bool AcceptTerms { get; set; }
    
    public string VerificationToken { get; set; }
    
    public DateTime? Verified { get; set; }
    
    public bool IsVerified => Verified.HasValue || PasswordResetTime.HasValue;
    
    public string ResetToken { get; set; }
    
    public DateTime? ResetTokenExpires { get; set; }
    
    public DateTime? PasswordResetTime { get; set; }
    
    public Country Country { get; set; }

    public City City { get; set; }
    
    public List<RefreshToken> RefreshTokens { get; set; }

    public IList<GenealogicalTree> AvailableTrees { get; set; }

    public IList<GenealogicalTree> PersonalTrees { get; set; }

    public IList<AccountConnection> OutgoingConnections { get; set; }

    public IList<AccountConnection> IncomingConnections { get; set; }

    public IList<Person> OwnedPersons { get; set; }

    public bool HasToken(string token) 
    {
        return this.RefreshTokens?.Find(rt => rt.Token == token) is not null;
    }

    public Person GetRootPerson() => OwnedPersons?.FirstOrDefault(p => p.HasLinkToAccount);
}