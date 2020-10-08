namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Application.Identity;
    using Aviant.DDD.Application.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using IdentityResult = Aviant.DDD.Application.Identity.IdentityResult;

    public sealed class IdentityService : IIdentityService //TODO: This requires a major refactor
    {
        private readonly IConfiguration _config;

        private readonly UserManager<AccountUser> _userManager;

        public IdentityService(
            UserManager<AccountUser> userManager,
            IConfiguration           config)
        {
            _userManager = userManager;
            _config      = config;
        }

        #region IIdentityService Members

        public async Task<object?> AuthenticateAsync(
            string            username,
            string            password,
            CancellationToken cancellationToken = default)
        {
            // Check if user with that username exists
            var user = await _userManager.Users.FirstOrDefaultAsync(
                    u =>
                        u.UserName == username,
                    cancellationToken)
               .ConfigureAwait(false);

            // Check if the user exists
            if (user is null) return null;

            // Check if the user is locked out
            if (_userManager.SupportsUserLockout
             && await _userManager.IsLockedOutAsync(user).ConfigureAwait(false))
                return new { error = "Account is locked" };

            // Check if the provided password is correct
            if (!await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
            {
                // Lock user
                if (_userManager.SupportsUserLockout
                 && await _userManager.GetLockoutEnabledAsync(user).ConfigureAwait(false))
                    await _userManager.AccessFailedAsync(user).ConfigureAwait(false);

                return new { error = "Invalid credentials" };
            }

            // Reset user count
            if (_userManager.SupportsUserLockout
             && 0 < await _userManager.GetAccessFailedCountAsync(user).ConfigureAwait(false))
                await _userManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);

            // Check if email has been confirmed
            if (!user.EmailConfirmed)
                return new
                {
                    error = "Confirm your email first",
                    confirm_token = HttpUtility.UrlEncode(
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(
                                await _userManager.GenerateEmailConfirmationTokenAsync(user)
                                   .ConfigureAwait(false))))
                };

            return new { token = GenerateJwtToken(user) };
        }

        public async Task<IdentityResult> ConfirmEmailAsync(
            string            token,
            string            email,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email)
               .ConfigureAwait(false);

            if (user is null)
                return IdentityResult.Failure(new[] { "Invalid" });

            if (user.EmailConfirmed)
                return IdentityResult.Failure(new[] { "Email already confirmed" });

            var result = await _userManager.ConfirmEmailAsync(user, token)
               .ConfigureAwait(false);

            if (!result.Succeeded)
                return IdentityResult.Failure(new[] { result.Errors.First().Description });

            return IdentityResult.Success();
        }

        public async Task<string> GetUserNameAsync(
            Guid              userId,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine(userId);

            var user = await _userManager.Users.FirstAsync(u => u.Id == userId, cancellationToken)
               .ConfigureAwait(false);

            return user.UserName;
        }

        public async Task<(IdentityResult Result, Guid UserId)> CreateUserAsync(
            string            username,
            string            password,
            CancellationToken cancellationToken = default)
        {
            var user = new AccountUser
            {
                UserName = username,
                Email    = username
            };

            var result = await _userManager.CreateAsync(user, password)
               .ConfigureAwait(false);

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<IdentityResult> DeleteUserAsync(
            Guid              userId,
            CancellationToken cancellationToken = default)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
                return await DeleteUserAsync(user, cancellationToken)
                   .ConfigureAwait(false);

            return IdentityResult.Success();
        }

        #endregion

        public async Task<IdentityResult> DeleteUserAsync(
            AccountUser       user,
            CancellationToken cancellationToken = default)
        {
            var result = await _userManager.DeleteAsync(user)
               .ConfigureAwait(false);

            return result.ToApplicationResult();
        }

        private string GenerateJwtToken(AccountUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials  = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,    user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,  user.Email),
                // new Claim("DateOfJoing", userInfo.DateOfJoing.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Expires            = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials,
                Audience           = _config["Jwt:Issuer"],
                Issuer             = _config["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}