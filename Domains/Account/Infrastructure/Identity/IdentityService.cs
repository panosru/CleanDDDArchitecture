namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity
{
    using System;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Application.Identity;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Core.Timing;
    using Core;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using IdentityResult = Aviant.DDD.Application.Identity.IdentityResult;

    public sealed class IdentityService : IIdentityService //TODO: This requires a major refactor
    {
        private readonly IAccountDomainConfiguration _config;

        private readonly UserManager<AccountUser> _userManager;

        public IdentityService(
            UserManager<AccountUser>   userManager,
            IAccountDomainConfiguration config)
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

            // Creating claims based on the system and user information
            Claim[] claims =
            {
                new(JwtRegisteredClaimNames.Sub, _config.GetValue("Jwt:Subject")),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, Clock.Now.ToString(CultureInfo.InvariantCulture)),
                new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            SymmetricSecurityKey tokenDecryptionKey = new(Encoding.ASCII.GetBytes(_config.GetValue("Jwt:Key256Bit")));
            SymmetricSecurityKey issuerSigningKey   = new(Encoding.ASCII.GetBytes(_config.GetValue("Jwt:Key512Bit")));
            SigningCredentials   credentials        = new(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature);

            EncryptingCredentials encryptingCredentials = new(
                tokenDecryptionKey,
                SecurityAlgorithms.Aes256KW,
                SecurityAlgorithms.Aes256CbcHmacSha512);

            JwtSecurityTokenHandler tokenHandler = new();

            var token = tokenHandler.CreateJwtSecurityToken(
                _config.GetValue("Jwt:Issuer"),
                _config.GetValue("Jwt:Audience"),
                new ClaimsIdentity(claims),
                expires: Clock.Now.AddMinutes(double.Parse(_config.GetValue("Jwt:ExpirationDurationInMunuts"))),
                signingCredentials: credentials,
                encryptingCredentials: encryptingCredentials,
                notBefore: Clock.Now,
                issuedAt: Clock.Now);

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
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

            return !result.Succeeded
                ? IdentityResult.Failure(new[] { result.Errors.First().Description })
                : IdentityResult.Success();
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
            AccountUser user = new()
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

            if (user is not null)
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
    }
}