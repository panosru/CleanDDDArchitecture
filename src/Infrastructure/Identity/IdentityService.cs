namespace CleanArchitecture.Infrastructure.Identity
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Aviant.DDD.Application;
    using IIdentityService = Aviant.DDD.Application.Identity.IService;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<object> Authenticate(string username, string password)
        {
            // Check if user with that username exists
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.UserName == username);
        
            // Check if the user exists
            if (user is null) return null;
            
            // Check if the user is locked out
            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                return new { error = "User is locked" };
            
            // Check if the provided password is correct
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                // Lock user
                if (_userManager.SupportsUserLockout && await _userManager.GetLockoutEnabledAsync(user))
                {
                    await _userManager.AccessFailedAsync(user);
                }
                
                return new {error = "Invalid credentials"};
            }

            // Reset user count
            if (_userManager.SupportsUserLockout && 0 < await _userManager.GetAccessFailedCountAsync(user))
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }
            
            // Check if email has been confirmed
            if (!user.EmailConfirmed)
                return new
                {
                    error = "Confirm your email first",
                    confirm_token = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        await _userManager.GenerateEmailConfirmationTokenAsync(user)))
                };
            
            return new { token = GenerateJwtToken(user) };
        
        }

        public async Task<Result> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result.Failure(new string[] { "Invalid" });

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return Result.Failure(new string[] { result.Errors.First().Description });

            return Result.Success();
        }

        public async Task<string> GetUserNameAsync(Guid userId)
        {
            Console.WriteLine(userId);
            
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user.UserName;
        }
        
        public async Task<(Result Result, Guid UserId)> CreateUserAsync(string username, string password)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = username,
            };

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<Result> DeleteUserAsync(Guid userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // new Claim("DateOfJoing", userInfo.DateOfJoing.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials,
                Audience = _config["Jwt:Issuer"],
                Issuer = _config["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
