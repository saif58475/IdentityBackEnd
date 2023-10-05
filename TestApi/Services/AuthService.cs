using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApi.ApplicationDbContext;
using TestApi.Models;

namespace TestApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IConfiguration _configuration;
        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        //For Login User  
        public async Task<AuthModel> Login(SignInModel model) 
        {
            var user = await this._userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthModel { Message = "Email Or Password is incorrect" };
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                return new AuthModel
                {
                    Message = "Success",
                    Email = user.Email,
                    IsAuthenticated = true, 
                    Roles = roles.ToList(),
                    Token = new JwtSecurityTokenHandler().WriteToken(await GenerateJwtToken(user)),
                    UserName = user.UserName
                };
            }
            catch
            {
                return new AuthModel { Message = "Something Wrong" };
            }

        }
        //For Registration User
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await this._userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is Already Registered" };
            if (await this._userManager.FindByEmailAsync(model.FirstName) is not null)
                return new AuthModel { Message = "FirstName is Already Registered" };
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,  
            };
            var result = await this._userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }
             await _userManager.AddToRoleAsync(user, "User"); 
            var jwtSecurityToken = await GenerateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Message = "Success",
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName
            };
        }

        //this will generate JWT in the signin or registeration 
        //##################################################################################################
        public async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid",user.Id),
                new Claim("name", user.FirstName)
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: credentials
            );
            return token;
        }
        //################################################################################################
    }
}
