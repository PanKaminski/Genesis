using Genesis.App.Contract.Authentication.ApiModels;
using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Implementation.Utils;
using Genesis.WebApi.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Genesis.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly JwtSettings jwtSettings;

        public AccountController(IAccountService accountService, IOptions<JwtSettings> jwtSettings)
        {
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            this.jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        private string Origin => Request?.Headers["origin"];

        [HttpPost]
        public ActionResult<AuthenticateResponse> Login(AuthenticateRequest model)
        {
            var authResult = accountService.Login(model.Email, model.Password);
            
            this.SetTokenCookie(authResult.RefreshToken);

            return Ok(authResult);
        }

        [HttpPost]
        public ActionResult<AuthenticateResponse> RefreshToken()
        {
            var authResult = accountService.RefreshToken(Request?.Cookies["refreshToken"]);
            
            this.SetTokenCookie(authResult.RefreshToken);

            return Ok(authResult);
        }

        [HttpPost]
        public IActionResult RevokeToken(VerificationRequest model)
        {
            var token = model?.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }
            
            accountService.RevokeToken(token);
            return Ok(new { message = "Token revoked" });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterRequest model)
        { 
            await accountService.RegisterAsync(model, Origin);

            return Ok(new { message = "Registration successful. Check your email for account verification" });;
        }

        [HttpPost]
        public IActionResult VerifyEmail(VerificationRequest verificationModel)
        {
            accountService.VerifyEmail(verificationModel.Token);

            return Ok(new { message = "Verification successful" });
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest model)
        {
            await accountService.ForgotPasswordAsync(model.Email, Origin);

            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost]
        public IActionResult ValidateResetToken(string token)
        {
            accountService.ValidateResetToken(token);

            return Ok(new { message = "Token is valid" });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            accountService.ResetPassword(model.Token, model.Password);

            return Ok(new { message = "Password reset successful, you can now login" });
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(jwtSettings.RtExpireTimeDays),
                HttpOnly = true,
            };
            
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
