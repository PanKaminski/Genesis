using Genesis.Common.Enums;
using Newtonsoft.Json;

namespace Genesis.App.Contract.Authentication.ApiModels;

public class AuthenticateResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Picture { get; set; }

    public IEnumerable<Role> Roles { get; set; }

    public bool IsVerified { get; set; }

    public string JwtToken { get; set; }

    [JsonIgnore]
    public string RefreshToken { get; set; }
}