namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using Application.Identity;
    using Microsoft.IdentityModel.Tokens;

    public sealed class TokenService : ITokenService
    {
        private const double ExpiryDurationMinutes = 30;

        /// <inheritdoc />
        public string BuildToken(
            NameValueCollection     keys,
            string      subject,
            string      issuer,
            string      audience,
            AccountUser user)
        {
            // Creating claims based on the system and user information
            Claim[] claims =
            {
                new(JwtRegisteredClaimNames.Sub, subject),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            SymmetricSecurityKey tokenDecryptionKey = new(Encoding.ASCII.GetBytes(keys["Key256Bit"]!));
            SymmetricSecurityKey issuerSigningKey   = new(Encoding.ASCII.GetBytes(keys["Key512Bit"]!));
            SigningCredentials   credentials        = new(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature);

            EncryptingCredentials encryptingCredentials = new(
                tokenDecryptionKey,
                SecurityAlgorithms.Aes256KW,
                SecurityAlgorithms.Aes256CbcHmacSha512);

            JwtSecurityTokenHandler tokenHandler = new();

            var token = tokenHandler.CreateJwtSecurityToken(
                issuer,
                audience,
                new ClaimsIdentity(claims),
                expires: DateTime.UtcNow.AddMinutes(ExpiryDurationMinutes),
                signingCredentials: credentials,
                encryptingCredentials: encryptingCredentials,
                notBefore: DateTime.UtcNow,
                issuedAt: DateTime.UtcNow);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <inheritdoc />
        public bool ValidateToken(
            NameValueCollection keys,
            string              issuer,
            string              audience,
            string              token)
        {
            SymmetricSecurityKey    tokenDecryptionKey = new(Encoding.ASCII.GetBytes(keys["Key256Bit"]!));
            SymmetricSecurityKey    issuerSigningKey   = new(Encoding.ASCII.GetBytes(keys["Key512Bit"]!));
            JwtSecurityTokenHandler tokenHandler       = new();

            try
            {
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidIssuer              = issuer,
                        ValidAudience            = audience,
                        TokenDecryptionKey       = tokenDecryptionKey,
                        IssuerSigningKey         = issuerSigningKey,
                        ClockSkew                = TimeSpan.Zero
                    },
                    out SecurityToken _);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}