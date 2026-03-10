using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Aviant.Core.Extensions;
using Aviant.Core.Timing;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;

internal sealed class RefreshSessionManager
{
    private const string SessionIdClaim = "sid";
    private const string TokenFamilyIdClaim = "family_id";

    private readonly AccountDbContextWrite _dbContext;
    private readonly IAccountDomainConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    internal RefreshSessionManager(
        AccountDbContextWrite accountDbContextWrite,
        IAccountDomainConfiguration config,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = accountDbContextWrite;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }

    internal async Task<AuthResult> IssueTokensAsync(
        AccountUser user,
        CancellationToken cancellationToken)
    {
        var now = UtcNow();
        var sessionId = Guid.NewGuid();
        var tokenFamilyId = sessionId;
        var authResult = CreateTokens(user, sessionId, tokenFamilyId, now);

        _dbContext.RefreshSessions.Add(
            new AccountRefreshSession
            {
                Id = sessionId,
                UserId = user.Id,
                TokenHash = HashToken(authResult.RefreshToken),
                TokenFamilyId = tokenFamilyId,
                CreatedAtUtc = now,
                ExpiresAtUtc = GetRefreshTokenExpiresAtUtc(now),
                CreatedByIp = GetRemoteIp(),
                UserAgent = GetUserAgent()
            });

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return authResult;
    }

    internal async Task<AuthResult?> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        RefreshTokenPrincipal? principal = ValidateRefreshToken(refreshToken);
        if (principal is null)
            return null;

        var now = UtcNow();
        var session = await _dbContext.RefreshSessions
            .SingleOrDefaultAsync(
                item => item.Id == principal.SessionId && item.UserId == principal.UserId,
                cancellationToken)
            .ConfigureAwait(false);

        if (session is null)
            return null;

        var presentedHash = HashToken(refreshToken);
        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(session.TokenHash),
                Convert.FromHexString(presentedHash)))
        {
            await RevokeFamilyAsync(session.UserId, session.TokenFamilyId, "Refresh token replay detected.", cancellationToken)
                .ConfigureAwait(false);
            return null;
        }

        if (session.RevokedAtUtc.HasValue || session.RotatedAtUtc.HasValue || session.ExpiresAtUtc <= now)
        {
            await RevokeFamilyAsync(session.UserId, session.TokenFamilyId, "Refresh token replay detected.", cancellationToken)
                .ConfigureAwait(false);
            return null;
        }

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(item => item.Id == principal.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (user is null || !user.EmailConfirmed || user.Status != AccountStatus.Active)
            return null;

        var nextSessionId = Guid.NewGuid();
        var authResult = CreateTokens(user, nextSessionId, session.TokenFamilyId, now);

        session.LastUsedAtUtc = now;
        session.RotatedAtUtc = now;
        session.RevokedAtUtc = now;
        session.ReplacedBySessionId = nextSessionId;
        session.Reason = "Rotated";

        user.LastAccessed = now;

        _dbContext.RefreshSessions.Add(
            new AccountRefreshSession
            {
                Id = nextSessionId,
                UserId = user.Id,
                TokenHash = HashToken(authResult.RefreshToken),
                TokenFamilyId = session.TokenFamilyId,
                CreatedAtUtc = now,
                ExpiresAtUtc = GetRefreshTokenExpiresAtUtc(now),
                CreatedByIp = GetRemoteIp(),
                UserAgent = GetUserAgent()
            });

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return authResult;
    }

    internal async Task<bool> RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        RefreshTokenPrincipal? principal = ValidateRefreshToken(refreshToken);
        if (principal is null)
            return false;

        var session = await _dbContext.RefreshSessions
            .SingleOrDefaultAsync(
                item => item.Id == principal.SessionId && item.UserId == principal.UserId,
                cancellationToken)
            .ConfigureAwait(false);

        if (session is null || session.TokenHash != HashToken(refreshToken))
            return false;

        if (!session.RevokedAtUtc.HasValue)
        {
            session.RevokedAtUtc = UtcNow();
            session.Reason = "Logged out";
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    internal async Task<int> RevokeAllRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var now = UtcNow();
        var sessions = await _dbContext.RefreshSessions
            .Where(item => item.UserId == userId && item.RevokedAtUtc == null)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var session in sessions)
        {
            session.RevokedAtUtc = now;
            session.Reason = "Logged out from all devices";
        }

        if (sessions.Count > 0)
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return sessions.Count;
    }

    internal async Task<IReadOnlyCollection<AccountSessionDto>> GetActiveSessionsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var now = UtcNow();

        return await _dbContext.RefreshSessions
            .Where(
                item =>
                    item.UserId == userId
                 && item.RevokedAtUtc == null
                 && item.RotatedAtUtc == null
                 && item.ExpiresAtUtc > now)
            .OrderByDescending(item => item.CreatedAtUtc)
            .Select(
                item =>
                    new AccountSessionDto
                    {
                        Id = item.Id,
                        CreatedAtUtc = item.CreatedAtUtc,
                        ExpiresAtUtc = item.ExpiresAtUtc,
                        LastUsedAtUtc = item.LastUsedAtUtc,
                        CreatedByIp = item.CreatedByIp,
                        UserAgent = item.UserAgent
                    })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    internal async Task<bool> RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var session = await _dbContext.RefreshSessions
            .SingleOrDefaultAsync(
                item => item.Id == sessionId && item.UserId == userId,
                cancellationToken)
            .ConfigureAwait(false);

        if (session is null)
            return false;

        if (!session.RevokedAtUtc.HasValue)
        {
            session.RevokedAtUtc = UtcNow();
            session.Reason = "Session revoked";
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    private async Task RevokeFamilyAsync(
        Guid userId,
        Guid tokenFamilyId,
        string reason,
        CancellationToken cancellationToken)
    {
        var now = UtcNow();
        var sessions = await _dbContext.RefreshSessions
            .Where(item => item.UserId == userId && item.TokenFamilyId == tokenFamilyId && item.RevokedAtUtc == null)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var session in sessions)
        {
            session.RevokedAtUtc = now;
            session.Reason = reason;
        }

        if (sessions.Count > 0)
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private AuthResult CreateTokens(
        AccountUser user,
        Guid sessionId,
        Guid tokenFamilyId,
        DateTime now)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var accessToken = tokenHandler.CreateJwtSecurityToken(
            BuildTokenDescriptor(CreateAccessClaims(user, sessionId, tokenFamilyId, now), false, now));
        var refreshToken = tokenHandler.CreateEncodedJwt(
            BuildTokenDescriptor(CreateRefreshClaims(user, sessionId, tokenFamilyId, now), true, now));

        return new AuthResult
        {
            TokenType = "Bearer",
            AccessToken = tokenHandler.WriteToken(accessToken),
            ExpiresIn = Math.Max(0, (long)(accessToken.ValidTo - now).TotalSeconds),
            RefreshToken = refreshToken
        };
    }

    private IEnumerable<Claim> CreateAccessClaims(
        AccountUser user,
        Guid sessionId,
        Guid tokenFamilyId,
        DateTime now)
    {
        return
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(SessionIdClaim, sessionId.ToString()),
            new(TokenFamilyIdClaim, tokenFamilyId.ToString())
        ];
    }

    private IEnumerable<Claim> CreateRefreshClaims(
        AccountUser user,
        Guid sessionId,
        Guid tokenFamilyId,
        DateTime now)
    {
        return
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new(SessionIdClaim, sessionId.ToString()),
            new(TokenFamilyIdClaim, tokenFamilyId.ToString())
        ];
    }

    private SecurityTokenDescriptor BuildTokenDescriptor(
        IEnumerable<Claim> claims,
        bool isRefreshToken,
        DateTime now)
    {
        var keyType = isRefreshToken ? "Refresh" : "Access";

        return new SecurityTokenDescriptor
        {
            Issuer = GetRequiredConfigurationValue("Jwt:Issuer"),
            Audience = GetRequiredConfigurationValue("Jwt:Audience"),
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddMinutes(
                double.Parse(
                    GetRequiredConfigurationValue($"Jwt:{keyType}:ExpirationDurationInMinutes"),
                    CultureInfo.InvariantCulture)),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(GetRequiredConfigurationValue($"Jwt:{keyType}:Key512Bit"))),
                SecurityAlgorithms.HmacSha256Signature),
            EncryptingCredentials = new EncryptingCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(GetRequiredConfigurationValue($"Jwt:{keyType}:Key256Bit"))),
                SecurityAlgorithms.Aes256KW,
                SecurityAlgorithms.Aes256CbcHmacSha512),
            NotBefore = now,
            IssuedAt = now
        };
    }

    private RefreshTokenPrincipal? ValidateRefreshToken(string refreshToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler
            {
                MapInboundClaims = false
            };

            var principal = tokenHandler.ValidateToken(
                refreshToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = GetRequiredConfigurationValue("Jwt:Issuer"),
                    ValidAudience = GetRequiredConfigurationValue("Jwt:Audience"),
                    TokenDecryptionKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(GetRequiredConfigurationValue("Jwt:Refresh:Key256Bit"))),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(GetRequiredConfigurationValue("Jwt:Refresh:Key512Bit"))),
                    ClockSkew = TimeSpan.FromMinutes(
                        double.Parse(GetRequiredConfigurationValue("Jwt:ClockSkewInMinutes"), CultureInfo.InvariantCulture))
                },
                out _);

            var userIdValue = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var sessionIdValue = principal.FindFirstValue(SessionIdClaim);
            var tokenFamilyIdValue = principal.FindFirstValue(TokenFamilyIdClaim);

            if (!Guid.TryParse(userIdValue, out var userId)
             || !Guid.TryParse(sessionIdValue, out var sessionId)
             || !Guid.TryParse(tokenFamilyIdValue, out var tokenFamilyId))
                return null;

            return new RefreshTokenPrincipal(userId, sessionId, tokenFamilyId);
        }
        catch
        {
            return null;
        }
    }

    private string GetRequiredConfigurationValue(string path) =>
        _config.Configuration()[path]
        ?? throw new InvalidOperationException(
            $"Missing required account configuration value '{path}'.");

    private DateTime GetRefreshTokenExpiresAtUtc(DateTime now) =>
        now.AddMinutes(
            double.Parse(
                GetRequiredConfigurationValue("Jwt:Refresh:ExpirationDurationInMinutes"),
                CultureInfo.InvariantCulture));

    private static DateTime UtcNow()
    {
        var now = Clock.Now;

        return now.Kind switch
        {
            DateTimeKind.Utc => now,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(now, DateTimeKind.Utc),
            _ => now.ToUniversalTime()
        };
    }

    private static string HashToken(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private string? GetRemoteIp() =>
        _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    private string? GetUserAgent()
    {
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

        if (string.IsNullOrWhiteSpace(userAgent))
            return null;

        return userAgent.Length <= 512 ? userAgent : userAgent[..512];
    }

    private sealed record RefreshTokenPrincipal(Guid UserId, Guid SessionId, Guid TokenFamilyId);
}
