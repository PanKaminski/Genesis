using System.Security.Cryptography;
using AutoMapper;
using Genesis.App.Contract.Authentication.ApiModels;
using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Models.Authentication;
using Genesis.App.Implementation.Utils;
using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.Common.Exceptions;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.Dtos.Account;
using Genesis.DAL.Contract.LoadOptions.Account;
using Genesis.DAL.Contract.UOW;
using Microsoft.Extensions.Options;

namespace Genesis.App.Implementation.Authentication.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ITokenManager tokenManager;
    private readonly IEmailService emailService;
    private readonly IMapper mapper;
    private readonly JwtSettings jwtSettings;

    public AccountService(IUnitOfWork unitOfWork, ITokenManager tokenManager, IEmailService emailService, 
        IMapper mapper, IOptions<JwtSettings> jwtSettings)
    {
        this.unitOfWork = unitOfWork;
        this.tokenManager = tokenManager;
        this.emailService = emailService;
        this.mapper = mapper;
        this.jwtSettings = jwtSettings.Value;
    }

    public AuthenticateResponse Login(string email, string password)
    {
        var accountDto = unitOfWork.AccountsRepository.GetByEmail(email, new List<AccountLoadOptions> { AccountLoadOptions.WithPersonData });
        
        if (accountDto is not { IsVerified: true })
        {
            throw new GenesisApplicationException($"Account {email} is not verified", nameof(email));
        }

        if (!BCrypt.Net.BCrypt.Verify(password, accountDto.PasswordHash))
        {
            throw new GenesisApplicationException("Password is incorrect", nameof(email));
        }

        var jwt = tokenManager.GenerateJwt(accountDto.Id, accountDto.Roles.Select(r => r.RoleName));
        var refreshToken = tokenManager.GenerateRefreshToken();

        accountDto.RefreshTokens.Add(mapper.Map<RefreshTokenDto>(refreshToken));
        unitOfWork.AccountsRepository.RemoveOldRefreshTokens(accountDto, jwtSettings.RtTokenTTLDays);

        unitOfWork.Commit();

        var response = mapper.Map<AuthenticateResponse>(accountDto);
        response.JwtToken = jwt;
        response.RefreshToken = refreshToken.Token;

        return response;
    }

    public AuthenticateResponse RefreshToken(string rtToken)
    {
        var accountDto = unitOfWork.AccountsRepository.GetByRefreshToken(rtToken);

        var rtTokenModel = accountDto.RefreshTokens.Single(rt => rt.Token == rtToken);

        if (rtTokenModel.Revoked is { })
        {
            RevokeAllChildTokens(rtTokenModel, accountDto, "Attempt to access resources by compromised token");
            unitOfWork.AccountsRepository.Update(accountDto);
        }
        
        if (!rtTokenModel.IsActive)
        {
            throw new GenesisApplicationException("Invalid refresh token");
        }

        var newRtToken = RotateRefreshToken(rtTokenModel);
        accountDto.RefreshTokens.Add(newRtToken);
        
        unitOfWork.AccountsRepository.RemoveOldRefreshTokens(accountDto, jwtSettings.RtTokenTTLDays);
        unitOfWork.Commit();

        var jwtToken = tokenManager.GenerateJwt(accountDto.Id, accountDto.Roles.Select(r => r.RoleName));

        var response = mapper.Map<AuthenticateResponse>(accountDto);
        response.JwtToken = jwtToken;
        response.RefreshToken = newRtToken.Token;

        return response;
    }

    public void RevokeToken(string token)
    {
        var accountDto = unitOfWork.AccountsRepository.GetByRefreshToken(token);

        if (accountDto is null)
        {
            throw new GenesisApplicationException("Invalid token.");
        }
        
        var refreshToken = accountDto.RefreshTokens.Single(rt => rt.Token == token);
        
        RevokeRefreshToken(refreshToken);

        unitOfWork.AccountsRepository.Update(accountDto);
        unitOfWork.Commit();
    }

    public async Task RegisterAsync(RegisterRequest viewModel, string origin)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (unitOfWork.AccountsRepository.Exists(viewModel.Email))
        {
            var errorMessage = "User with such email already exists. Use \"Forgot password\" to change password via email";
            throw new GenesisApplicationException(errorMessage);
        }

        var rootPerson = new PersonDto(
            viewModel.FirstName,
            viewModel.LastName,
            viewModel.MiddleName,
            viewModel.Gender
        );
        unitOfWork.PersonsRepository.Add(rootPerson);

        var accountDto = new AccountDto
        {
            Login = viewModel.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(viewModel.Password),
            VerificationToken = GenerateVerificationToken(),
            Verified = null,
            RootPerson = rootPerson,
        };

        await emailService.SendEmailAsync(CreateEmailVerificationMessage(accountDto, origin));

        unitOfWork.AccountsRepository.Create(accountDto);

        unitOfWork.GenealogicalTreesRepository.CreateInitialTree(rootPerson);

        await unitOfWork.CommitAsync();
    }

    public void VerifyEmail(string token)
    {
        var account = unitOfWork.AccountsRepository.GetByVerificationToken(token);

        if (account is null)
        {
            throw new GenesisApplicationException("Verification failed");
        }

        account.Verified = DateTime.UtcNow;
        account.VerificationToken = null;

        unitOfWork.Commit();
    }

    public async Task ForgotPasswordAsync(string email, string origin)
    {
        var accountDto = unitOfWork.AccountsRepository.GetByEmail(email, new List<AccountLoadOptions> { AccountLoadOptions.WithPersonData });

        if (accountDto is null) return;

        accountDto.ResetToken = GenerateResetToken();
        accountDto.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        await emailService.SendEmailAsync(CreatePasswordResetMessage(accountDto, origin));

        unitOfWork.Commit();
    }

    public void ResetPassword(string token, string password)
    {
        var account = GetAccountByResetToken(token);

        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        unitOfWork.Commit();
    }

    public void ValidateResetToken(string token) => GetAccountByResetToken(token);

    public async Task<PagedModel<Account>> GetConnections(int accountId, int page, int pageSize)
    {
        var accountsModel = await this.unitOfWork.AccountsRepository.GetConnections(
            accountId, ConnectionStatus.Accepted, true, true, page, pageSize,
            false, new List<AccountLoadOptions> { AccountLoadOptions.WithFullPersonData, AccountLoadOptions.WithConnections }
            );

        return new PagedModel<Account>
        {
            CurrentPage = accountsModel.CurrentPage,
            TotalCount = accountsModel.TotalCount,
            PageSize = accountsModel.PageSize,
            Items = mapper.Map<IEnumerable<Account>>(accountsModel.Items),
        };
    }

    public async Task<PagedModel<Account>> GetInvites(int accountId, int page, int pageSize)
    {
        var accountsModel = await this.unitOfWork.AccountsRepository.GetConnections(
            accountId, ConnectionStatus.Pending, false, true, page, pageSize,
            false, new List<AccountLoadOptions> { AccountLoadOptions.WithFullPersonData, AccountLoadOptions.WithConnections }
            );

        return new PagedModel<Account>
        {
            CurrentPage = accountsModel.CurrentPage,
            TotalCount = accountsModel.TotalCount,
            PageSize = accountsModel.PageSize,
            Items = mapper.Map<IEnumerable<Account>>(accountsModel.Items),
        };
    }

    public async Task<PagedModel<Account>> GetPendings(int accountId, int page, int pageSize)
    {
        var accountsModel = await this.unitOfWork.AccountsRepository.GetConnections(
            accountId, ConnectionStatus.Pending, true, false, page, pageSize,
            false, new List<AccountLoadOptions> { AccountLoadOptions.WithFullPersonData, AccountLoadOptions.WithConnections }
            );

        return new PagedModel<Account>
        {
            CurrentPage = accountsModel.CurrentPage,
            TotalCount = accountsModel.TotalCount,
            PageSize = accountsModel.PageSize,
            Items = mapper.Map<IEnumerable<Account>>(accountsModel.Items),
        };
    }

    public async Task<PagedModel<Account>> SearchUsers(int accountId, int page, int pageSize)
    {
        var accountsModel = await unitOfWork.AccountsRepository.SearchUsers(
                accountId, page, pageSize,
                false, new List<AccountLoadOptions> { AccountLoadOptions.WithFullPersonData, AccountLoadOptions.WithConnections }
                );

        return new PagedModel<Account>
        {
            CurrentPage = accountsModel.CurrentPage,
            TotalCount = accountsModel.TotalCount,
            PageSize = accountsModel.PageSize,
            Items = mapper.Map<IEnumerable<Account>>(accountsModel.Items),
        };
    }

    public IEnumerable<Account> GetConnections(int accountId)
    {
        var accountsModel = this.unitOfWork.AccountsRepository.GetConnections(
            accountId, ConnectionStatus.Accepted, true, true,
            false, new List<AccountLoadOptions> { AccountLoadOptions.WithFullPersonData, AccountLoadOptions.WithConnections }
            );

        return mapper.Map<IEnumerable<Account>>(accountsModel);
    }

    private EmailMessage CreateEmailVerificationMessage(AccountDto account, string origin)
    {
        var message = "<h1>Thank you for registering!</h1>\n";

        if (!string.IsNullOrEmpty(origin))
        {
            var verifyUrl = $"{origin}/verify-email?token={account.VerificationToken}";

            message += $@"<p>Please follow the link to verify your email address:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        }
        else
        {
            message += $@"<p>Please insert this token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                            <p><code>{account.VerificationToken}</code></p>";
        }

        var targetName = $"{account.RootPerson.GetFullName()}";

        return new EmailMessage("Genesis - Verify Email", message, account.Login, targetName);
    }

    private EmailMessage CreatePasswordResetMessage(AccountDto account, string origin)
    {
        var message = "<h4>Reset Password Email</h4>\n";
        if (!string.IsNullOrEmpty(origin))
        {
            var resetUrl = $"{origin}/reset-password?token={account.ResetToken}";
            message += $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        }
        else
        {
            message += $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                            <p><code>{account.ResetToken}</code></p>";
        }

        var targetName = account.RootPerson.GetFullName();

        return new EmailMessage("Genesis - Reset Password", message, account.Login, targetName);
    }


    private AccountDto GetAccountByResetToken(string token) => unitOfWork.AccountsRepository.GetByActiveResetToken(token);

    private string GenerateResetToken()
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        var isUniqueToken = !unitOfWork.AccountsRepository.TryGetSingle(a => a.ResetToken == token, out _);

        return !isUniqueToken ? GenerateResetToken() : token;
    }

    private string GenerateVerificationToken()
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        var isUniqueToken = !unitOfWork.AccountsRepository.TryGetSingle(a => a.VerificationToken == token, out _);

        return !isUniqueToken ? GenerateResetToken() : token;
    }

    private void RevokeAllChildTokens(RefreshTokenDto rtToken, AccountDto account, string reason = null)
    {
        if (string.IsNullOrEmpty(rtToken.ReplacedByToken)) return;
        
        var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == rtToken.ReplacedByToken);
            
        if (childToken is null) return;
            
        if (childToken.IsActive)
            RevokeRefreshToken(childToken, reason);
        else
            RevokeAllChildTokens(childToken, account, reason);
    }

    private RefreshTokenDto RotateRefreshToken(RefreshTokenDto refreshToken)
    {
        var newRefreshToken = tokenManager.GenerateRefreshToken();
        RevokeRefreshToken(refreshToken, "Replaced by new token", newRefreshToken.Token);

        return mapper.Map<RefreshTokenDto>(newRefreshToken);
    }

    private void RevokeRefreshToken(RefreshTokenDto token, string reason = null, string replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }
}