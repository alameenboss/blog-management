using Applogiq.IdentityServer.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Applogiq.IdentityServer.JwtFeatures
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfigurationSection _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtHandler(IConfiguration configuration,
           RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            this.roleManager = roleManager;
            _jwtSettings = _configuration.GetSection("JwtSettings");
        }

        private SigningCredentials GetSigningCredentials()
        {
            byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value);
            SymmetricSecurityKey secret = new(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            List<Claim> claims = new()
            {
                new Claim("email", user.Email),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("id", user.Id)
            };

            IList<string> roles = await _userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim("role", role));

                await AddRoleClaimsAsync(claims, role);
            }

            return claims;
        }

        private async Task AddRoleClaimsAsync(List<Claim> claims, string roleName)
        {
            IdentityRole? r = await roleManager.FindByNameAsync(roleName);
            IList<Claim> rClaims = await roleManager.GetClaimsAsync(r);
            rClaims.ToList().ForEach(c =>
            {
                claims.Add(new Claim(c.Type, c.Value));
            });
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            JwtSecurityToken tokenOptions = new(
                issuer: _jwtSettings.GetSection("validIssuer").Value,
                audience: _jwtSettings.GetSection("validAudience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.GetSection("expiryInMinutes").Value)),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            SigningCredentials signingCredentials = GetSigningCredentials();
            List<Claim> claims = await GetClaims(user);
            JwtSecurityToken tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return token;
        }
    }
}