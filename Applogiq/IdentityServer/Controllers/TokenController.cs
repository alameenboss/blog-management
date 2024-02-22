using Applogiq.IdentityServer.DTOs.Auth;
using Applogiq.IdentityServer.DTOs.User;
using Applogiq.IdentityServer.JwtFeatures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Applogiq.IdentityServer.Controllers
{

    public class TokenController : DefaultBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHandler _jwtHandler;

        public TokenController(
            UserManager<ApplicationUser> userManager,

            JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
        }

        
        [HttpPost]
        public async Task<IActionResult> TokenAsync(LoginDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.Email);
            if (user == null)
            {
                return BadRequest("Invalid Request");
            }

            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            {
                return BadRequest("Invalid Password");
            }

            string token = await _jwtHandler.GenerateTokenAsync(user);

            return Ok(token);
        }
    }
}