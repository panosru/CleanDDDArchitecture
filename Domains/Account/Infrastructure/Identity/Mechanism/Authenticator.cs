using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Aviant.Core.Extensions;
using Aviant.Core.Timing;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;

/// <summary>
/// Handle user authentication
/// </summary>
internal class Authenticator
{
    private readonly IAccountDomainConfiguration _config;

    private readonly UserManager<AccountUser> _userManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="config"></param>
    internal Authenticator(
        UserManager<AccountUser>    userManager,
        IAccountDomainConfiguration config)
    {
        _userManager = userManager;
        _config      = config;
    }
    
    /// <summary>
    /// Main method to handle user authentication
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<object?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        // Locate user by username
        var user = await FindUserAsync(username, cancellationToken);
        if (user == null)
            return null;

        // Check if user is locked out
        if (await IsUserLockedOutAsync(user))
            return new { error = "Account is locked" };

        // Validate password
        if (!await CheckPasswordAsync(user, password))
            return new { error = "Invalid credentials" };

        // Check if user's email is confirmed
        if (!user.EmailConfirmed)
            return new
            {
                error = "Confirm your email first",
                confirm_token = await GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false)
            };

        // Reset access failed count if necessary
        await ResetAccessFailedCountIfNeededAsync(user);

        // Generate JWT token
        var (accessToken, accessTokenSerialised, refreshTokenSerialised) = GenerateJwtTokens(user);
        
        // Update last accessed date
        await UpdateLastAccessedAsync(user);

        return new AuthResult
        {
            TokenType = "Bearer",
            AccessToken = accessTokenSerialised,
            ExpiresIn = (long)(accessToken.ValidTo.ToUnixTimestamp() - Clock.Now.ToUnixTimestamp()),
            RefreshToken = refreshTokenSerialised
        };
    }
    
    /// <summary>
    /// Find a user by username
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<AccountUser?> FindUserAsync(string username, CancellationToken cancellationToken)
    {
        return await _userManager.Users.FirstOrDefaultAsync(
            u => u.UserName == username, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Check if user is locked out
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<bool> IsUserLockedOutAsync(AccountUser user)
    {
        return _userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Validate user password
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private async Task<bool> CheckPasswordAsync(AccountUser user, string password)
    {
        // Check if the provided password is correct
        if (await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
            return true;

        // Process failed access attempt
        if (_userManager.SupportsUserLockout 
            && await _userManager.GetLockoutEnabledAsync(user).ConfigureAwait(false))
            await _userManager.AccessFailedAsync(user).ConfigureAwait(false);

        return false;
    }

    /// <summary>
    /// Generate token for email confirmation
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<string> GenerateEmailConfirmationTokenAsync(AccountUser user)
    {
        return HttpUtility.UrlEncode(
            Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    await _userManager.GenerateEmailConfirmationTokenAsync(user)
                        .ConfigureAwait(false))));
    }
    
    /// <summary>
    /// Reset the access failed count if necessary
    /// </summary>
    /// <param name="user"></param>
    private async Task ResetAccessFailedCountIfNeededAsync(AccountUser user)
    {
        if (_userManager.SupportsUserLockout 
            && 0 < await _userManager.GetAccessFailedCountAsync(user).ConfigureAwait(false))
            await _userManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Generate JWT token for the user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private (JwtSecurityToken, string, string) GenerateJwtTokens(AccountUser user)
    {
        // Create the JWT token handler
        var tokenHandler = new JwtSecurityTokenHandler();
        
        // Get the claims for the JWT tokens
        var accessClaims = CreateAccessClaims(user);
        var refreshClaims = CreateRefreshClaims(user);
        
        // Build the token descriptors for the JWT tokens
        var accessTokenDescriptor = BuildTokenDescriptor(accessClaims);
        var refreshTokenDescriptor = BuildTokenDescriptor(refreshClaims, true);

        // Generate the JWT tokens
        var accessToken = tokenHandler.CreateJwtSecurityToken(accessTokenDescriptor);
        var refreshToken = tokenHandler.CreateEncodedJwt(refreshTokenDescriptor);
        
        // Return the JWT token object and a serialised version to a string format
        return (accessToken, tokenHandler.WriteToken(accessToken), refreshToken);
    }
    
    /// <summary>
    /// Create claims for the JWT access token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private IEnumerable<Claim> CreateAccessClaims(AccountUser user)
    {
        return new Claim[]
        {
            // Subject (sub) claim: Unique identifier for the user (e.g., user ID).
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            // JWT ID (jti) claim: Unique identifier for this token.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Issued At (iat) claim: Time at which the JWT was issued.
            new(JwtRegisteredClaimNames.Iat, 
                Clock.Now.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture), 
                ClaimValueTypes.Integer64),
            
            // Name ID (nameid) claim: Name that identifies the user.
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),

            // User Name claim: Could be used to store the username.
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),

            // Email claim: User's email address.
            new(JwtRegisteredClaimNames.Email, user.Email),

            // Given Name claim: User's first name, if available.
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),

            // Family Name claim: User's last name, if available.
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),

            // Additional claims can be added here as needed, for example:
            // Role claim: To include the user's role.
            // new Claim(ClaimTypes.Role, user.Role)
        };
    }
    
    /// <summary>
    /// Create claims for the JWT refresh token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private IEnumerable<Claim> CreateRefreshClaims(AccountUser user)
    {
        return new Claim[]
        {
            // Subject (sub) claim: Unique identifier for the user.
            // This is the most important claim for a refresh token, as it links the token to the user.
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            // JWT ID (jti) claim: Unique identifier for this token.
            // Useful for tracking and invalidating individual tokens if necessary.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Issued At (iat) claim: Time at which the JWT was issued.
            // Helps in determining the age of the token.
            new(JwtRegisteredClaimNames.Iat, 
                Clock.Now.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture), 
                ClaimValueTypes.Integer64),

            // You may add any other claims that are necessary for refreshing tokens.
            // Typically, these would be minimal to ensure the refresh token remains lightweight.
        };
    }

    
    /// <summary>
    /// Build the token descriptor for the JWT
    /// </summary>
    /// <param name="claims"></param>
    /// <param name="isRefreshToken"></param>
    /// <returns></returns>
    private SecurityTokenDescriptor BuildTokenDescriptor(IEnumerable<Claim> claims, bool isRefreshToken = false)
    {
        var keyType = isRefreshToken ? "Refresh" : "Access";
        
        SymmetricSecurityKey tokenDecryptionKey = new(
            Encoding.ASCII.GetBytes(_config.GetValue($"Jwt:{keyType}:Key256Bit")));
        
        SymmetricSecurityKey issuerSigningKey = new(
            Encoding.ASCII.GetBytes(_config.GetValue($"Jwt:{keyType}:Key512Bit")));

        return new SecurityTokenDescriptor
        {
            // "Issuer" represents the issuer of the token (the entity that created and signed the token).
            Issuer = _config.GetValue("Jwt:Issuer"),

            // "Audience" represents the intended recipient or recipients of the token.
            Audience = _config.GetValue("Jwt:Audience"),

            // "Subject" represents the principal that is the subject of the JWT. The claims in a
            // JWT are normally statements about the subject.
            Subject = new ClaimsIdentity(claims),

            // "Expires" sets the time at which the token will expire. It's usually a specific time in the future.
            Expires = Clock.Now.AddMinutes(double.Parse(
                _config.GetValue($"Jwt:{keyType}:ExpirationDurationInMinutes"), CultureInfo.InvariantCulture)),

            // "SigningCredentials" are used to create a security token signature.
            SigningCredentials = new SigningCredentials(issuerSigningKey, 
                SecurityAlgorithms.HmacSha256Signature),

            // "EncryptingCredentials" are used to encrypt the token.
            EncryptingCredentials = new EncryptingCredentials(tokenDecryptionKey, 
                SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512),

            // "NotBefore" sets the time before which the JWT must not be accepted for processing.
            NotBefore = Clock.Now,

            // "IssuedAt" sets the time at which the JWT was issued.
            IssuedAt = Clock.Now
        };
    }

    /// <summary>
    /// Update the user's last accessed date
    /// </summary>
    /// <param name="user"></param>
    private async Task UpdateLastAccessedAsync(AccountUser user)
    {
        user.LastAccessed = Clock.Now;
        await _userManager.UpdateAsync(user).ConfigureAwait(false);
    }
}