﻿namespace Genesis.App.Contract.Models.Authentication;

public class RefreshToken
{
    public int Id { get; set; }
    
    public Account Account { get; set; }
    
    public string Token { get; set; }
    
    public DateTime Expires { get; set; }
    
    public DateTime Created { get; set; }
    
    public string CreatedByIp { get; set; }
    
    public DateTime? Revoked { get; set; }
    
    public string RevokedByIp { get; set; }
    
    public string ReplacedByToken { get; set; }
    
    public string ReasonRevoked { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= Expires;
    
    public bool IsRevoked => Revoked is not null;
    
    public bool IsActive => Revoked is null && !IsExpired;
}
