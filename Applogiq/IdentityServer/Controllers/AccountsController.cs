using Applogiq.EmailService;
using Applogiq.EmailService.Config;
using Applogiq.EmailService.Model;
using Applogiq.IdentityServer.Constants;
using Applogiq.IdentityServer.DTOs.Auth;
using Applogiq.IdentityServer.DTOs.User;
using Applogiq.IdentityServer.JwtFeatures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Applogiq.IdentityServer.Controllers
{

    public class AccountsController : DefaultBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailSender _emailSender;

        public AccountsController(
            UserManager<ApplicationUser> userManager,

            JwtHandler jwtHandler,

            IEmailSender emailSender)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _emailSender = emailSender;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            ApplicationUser user = _mapper.Map<ApplicationUser>(userForRegistration);
            user.Enabled = true;
            IdentityResult result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded)
            {
                IEnumerable<string> errors = result.Errors.Select(e => e.Description);

                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }
            await _userManager.AddToRoleAsync(user, RoleConstant.MemberRoleName);


            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            Dictionary<string, string> param = new()
            {
                {"token", token },
                {"email", user.Email }
            };

            string callback = QueryHelpers.AddQueryString(userForRegistration.ClientURI, param);


            string content = $"Hi ${user.UserName}" +
                $"" +
                $"We're excited to have you get started. First, you need to confirm your email - ${user.Email} for this account. Copy and paste the following link in your browser:\r\n\r\n" +
                $"${callback}";

            Message message = new(new EmailAddress(user.Email, user.Email),
                "Email Confirmation token",
                content,
                null);
            await _emailSender.SendEmailAsync(message);

            return StatusCode(201);
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userForAuthentication.Email);
            if (user == null)
            {
                return BadRequest("Invalid Request");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed" });
            }

            //you can check here if the account is locked out in case the user enters valid credentials after locking the account.

            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            {
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    string content = $"Your account is locked out. To reset the password click this link: {userForAuthentication.ClientURI}";
                    Message message = new(new EmailAddress(userForAuthentication.Email, userForAuthentication.Email), "Locked out account information", content, null);
                    await _emailSender.SendEmailAsync(message);

                    return Unauthorized(new AuthResponseDto { ErrorMessage = "The account is locked out" });
                }

                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return await GenerateOTPFor2StepVerification(user);
            }

            string token = await _jwtHandler.GenerateTokenAsync(user);

            await _userManager.ResetAccessFailedCountAsync(user);

            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }

        private async Task<IActionResult> GenerateOTPFor2StepVerification(ApplicationUser user)
        {
            IList<string> providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid 2-Step Verification Provider." });
            }

            string token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            string content = $"You two factor token is <h2>{token}</h2>";

            Message message = new(new EmailAddress(user.Email, user.Email), "Authentication token", content, null);
            await _emailSender.SendEmailAsync(message);

            return Ok(new AuthResponseDto { Is2StepVerificationRequired = true, Provider = "Email" });
        }

        [HttpPost("TwoStepVerification")]
        public async Task<IActionResult> TwoStepVerification([FromBody] TwoFactorDto twoFactorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(twoFactorDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid Request");
            }

            bool validVerification = await _userManager.VerifyTwoFactorTokenAsync(user, twoFactorDto.Provider, twoFactorDto.Token);
            if (!validVerification)
            {
                return BadRequest("Invalid Token Verification");
            }

            string token = await _jwtHandler.GenerateTokenAsync(user);
            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid Request");
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            Dictionary<string, string> param = new()
            {
                {"token", token },
                {"email", forgotPasswordDto.Email }
            };

            string callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);
            string content = $"Hi ${user.FirstName + " " + user.LastName}! We noticed that you recently requested to reset your password for your account. Click on the following link to reset your password ${callback}";


            Message message = new(new EmailAddress(user.Email, user.Email), "Reset password", content, null);
            await _emailSender.SendEmailAsync(message);

            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid Request");
            }

            IdentityResult resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            if (!resetPassResult.Succeeded)
            {
                IEnumerable<string> errors = resetPassResult.Errors.Select(e => e.Description);

                return BadRequest(new { Errors = errors });
            }

            await _userManager.SetLockoutEndDateAsync(user, new DateTime(2000, 1, 1));

            return Ok();
        }

        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid Email Confirmation Request");
            }

            IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            return !confirmResult.Succeeded ? BadRequest("Invalid Email Confirmation Request") : Ok();
        }
    }
}