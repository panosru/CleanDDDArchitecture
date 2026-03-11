using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;

internal sealed class TrustedDeviceManager
{
    private readonly AccountDbContextWrite _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccountDomainConfiguration _config;

    internal TrustedDeviceManager(
        AccountDbContextWrite dbContext,
        IHttpContextAccessor httpContextAccessor,
        IAccountDomainConfiguration config)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _config = config;
    }

    internal async Task<string> IssueTrustedDeviceTokenAsync(
        Guid userId,
        string? deviceName,
        CancellationToken cancellationToken)
    {
        var deviceId = Guid.NewGuid();
        var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var now = DateTimeOffset.UtcNow;

        _dbContext.TrustedDevices.Add(
            new AccountTrustedDevice
            {
                Id = deviceId,
                UserId = userId,
                TokenHash = HashSecret(secret),
                DeviceName = string.IsNullOrWhiteSpace(deviceName) ? null : deviceName.Trim(),
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(GetExpirationDurationInDays()),
                CreatedByIp = GetRemoteIp(),
                UserAgent = GetUserAgent()
            });

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return $"{deviceId:N}.{secret}";
    }

    internal async Task<bool> IsTrustedDeviceAsync(
        Guid userId,
        string? trustedDeviceToken,
        CancellationToken cancellationToken)
    {
        if (!TryParseToken(trustedDeviceToken, out var deviceId, out var secret))
            return false;

        var device = await _dbContext.TrustedDevices
            .SingleOrDefaultAsync(item => item.Id == deviceId && item.UserId == userId, cancellationToken)
            .ConfigureAwait(false);

        if (device is null || device.RevokedAtUtc.HasValue || device.ExpiresAtUtc <= DateTimeOffset.UtcNow)
            return false;

        var expectedHash = Convert.FromHexString(device.TokenHash);
        var presentedHash = Convert.FromHexString(HashSecret(secret));

        if (!CryptographicOperations.FixedTimeEquals(expectedHash, presentedHash))
            return false;

        device.LastUsedAtUtc = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return true;
    }

    internal async Task<IReadOnlyCollection<AccountTrustedDeviceDto>> GetTrustedDevicesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var devices = await _dbContext.TrustedDevices
            .Where(item => item.UserId == userId && item.RevokedAtUtc == null && item.ExpiresAtUtc > now)
            .OrderByDescending(item => item.LastUsedAtUtc ?? item.CreatedAtUtc)
            .Select(
                item =>
                    new
                    {
                        Id = item.Id,
                        DeviceName = item.DeviceName,
                        item.CreatedAtUtc,
                        item.ExpiresAtUtc,
                        item.LastUsedAtUtc,
                        CreatedByIp = item.CreatedByIp,
                        UserAgent = item.UserAgent
                    })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return devices.Select(
                item =>
                    new AccountTrustedDeviceDto
                    {
                        Id = item.Id,
                        DeviceName = item.DeviceName,
                        CreatedAtUtc = item.CreatedAtUtc.UtcDateTime,
                        ExpiresAtUtc = item.ExpiresAtUtc.UtcDateTime,
                        LastUsedAtUtc = item.LastUsedAtUtc?.UtcDateTime,
                        CreatedByIp = item.CreatedByIp,
                        UserAgent = item.UserAgent
                    })
            .ToArray();
    }

    internal async Task<bool> RevokeTrustedDeviceAsync(
        Guid userId,
        Guid trustedDeviceId,
        CancellationToken cancellationToken)
    {
        var device = await _dbContext.TrustedDevices
            .SingleOrDefaultAsync(item => item.Id == trustedDeviceId && item.UserId == userId, cancellationToken)
            .ConfigureAwait(false);

        if (device is null)
            return false;

        if (!device.RevokedAtUtc.HasValue)
        {
            device.RevokedAtUtc = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    internal async Task<int> RevokeAllTrustedDevicesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var devices = await _dbContext.TrustedDevices
            .Where(item => item.UserId == userId && item.RevokedAtUtc == null)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var device in devices)
            device.RevokedAtUtc = now;

        if (devices.Count > 0)
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return devices.Count;
    }

    private int GetExpirationDurationInDays()
    {
        if (int.TryParse(
                _config.Configuration()["TrustedDevices:ExpirationDurationInDays"],
                CultureInfo.InvariantCulture,
                out var days)
         && days > 0)
            return days;

        return 30;
    }

    private static string HashSecret(string secret) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(secret)));

    private static bool TryParseToken(
        string? trustedDeviceToken,
        out Guid deviceId,
        out string secret)
    {
        deviceId = Guid.Empty;
        secret = string.Empty;

        if (string.IsNullOrWhiteSpace(trustedDeviceToken))
            return false;

        var parts = trustedDeviceToken.Split('.', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 || !Guid.TryParseExact(parts[0], "N", out deviceId))
            return false;

        secret = parts[1];
        return !string.IsNullOrWhiteSpace(secret);
    }

    private string? GetRemoteIp() => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    private string? GetUserAgent() => _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
}
