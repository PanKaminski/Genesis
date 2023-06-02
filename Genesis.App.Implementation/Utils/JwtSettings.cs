namespace Genesis.App.Implementation.Utils;

public class JwtSettings
{
    public string SecretKey { get; set; }

    public int JwtExpireTimeMinutes { get; set; }
    
    public int RtExpireTimeDays { get; set; }

    public int RtTokenTTLDays { get; set; }
}