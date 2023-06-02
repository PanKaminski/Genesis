using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Models.Authentication;
using Genesis.App.Implementation.Utils;
using Genesis.Common.Enums;
using Genesis.DAL.Contract.UOW;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Genesis.App.Implementation.Authentication.Services;

public class TokenManager : ITokenManager
{
    private readonly JwtSettings jwtSettings;
    private readonly IUnitOfWork unitOfWork;

    public TokenManager(IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork)
    {
        this.jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public string GenerateJwt(int accountId, IEnumerable<Role> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", accountId.ToString()),
                new Claim(ClaimTypes.Role, string.Join(',', roles.Select(r => Enum.GetName(r))))
            }),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.JwtExpireTimeMinutes),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            Issuer = "https://localhost:8181",
            Audience = "https://localhost:8181",
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public bool TryValidateToken(string token, out int accountId)
    {
        if (token is null)
        {
            accountId = default;
            return false;
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey));
        
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            
            return true;
        }
        catch
        {
            accountId = default;
            return false;
        }
    }
    
    public RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(jwtSettings.RtExpireTimeDays),
            Created = DateTime.UtcNow,
        };

        var tokenIsUnique = !unitOfWork.AccountsRepository.Get()
            .Any(a => a.RefreshTokens.Any(t => t.Token == refreshToken.Token));

        return !tokenIsUnique ? GenerateRefreshToken() : refreshToken;
    }
}