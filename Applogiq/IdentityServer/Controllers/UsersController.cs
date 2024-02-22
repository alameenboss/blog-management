using Applogiq.EmailService;
using Applogiq.EmailService.Config;
using Applogiq.EmailService.Model;
using Applogiq.IdentityServer.Constants;
using Applogiq.IdentityServer.DTOs.User;
using Applogiq.IdentityServer.EFConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Applogiq.IdentityServer.Controllers
{
    [Authorize(Roles = RoleConstant.SuperAdminRoleName)]
    public class UsersController : DefaultBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly UserDbContext _dbContext;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
             IEmailSender emailSender,
            UserDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<UserResponseDTO>> GetAllUsers()
        {
            List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();
            List<IdentityRole> allRoles = await _dbContext.Roles.ToListAsync();
            List<IdentityUserRole<string>> allUserRoles = await _dbContext.UserRoles.ToListAsync();
            List<UserResponseDTO> result = new();
            allUsers.ForEach(user =>
            {
                IEnumerable<string> userRoleIds = allUserRoles.Where(u => u.UserId == user.Id).Select(r => r.RoleId);
                IEnumerable<IdentityRole> userRoles = allRoles.Where(r => userRoleIds.Contains(r.Id));

                result.Add(new UserResponseDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Enabled = user.Enabled,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = userRoles.Select(x => x.Name).ToList()
                });

            });


            return result;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponseDTO>> GetUsersById(string userId)
        {

            ApplicationUser? user = await _userManager
                .Users
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }

            List<IdentityRole> allRoles = await _dbContext.Roles.ToListAsync();
            List<IdentityUserRole<string>> allUserRoles = await _dbContext.UserRoles.ToListAsync();

            IEnumerable<string> userRoleIds = allUserRoles.Where(u => u.UserId == user.Id).Select(r => r.RoleId);
            IEnumerable<IdentityRole> userRoles = allRoles.Where(r => userRoleIds.Contains(r.Id));

            UserResponseDTO result = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Enabled = user.Enabled,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Roles = userRoles.Select(x => x.Name).ToList()
            };

            return Ok(result);
        }



        [HttpPost]
        public async Task<ActionResult> AddUser(CreateUserDTO model)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                return BadRequest("Email Id is already registered,try with new email");
            }

            user = _mapper.Map<ApplicationUser>(model);
            user.Enabled = true;


            IdentityResult userCreatedResult = await _userManager.CreateAsync(user);

            if (userCreatedResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.Roles);
            }

            string emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string resetpasswordtoken = await _userManager.GeneratePasswordResetTokenAsync(user);
            Dictionary<string, string> param = new()
            {
                {"token", emailToken },
                {"email", user.Email },
                {"resetpasswordtoken", resetpasswordtoken  }
            };

            string callback = QueryHelpers.AddQueryString("http://localhost:4200/authentication/emailconfirmation", param);

            string content = $"Hi ${user.UserName}" +
              $"" +
              $"We're excited to have you get started. First, you need to confirm your email - ${user.Email} for this account. Copy and paste the following link in your browser:\r\n\r\n" +
              $"${callback}";

            Message message = new(new EmailAddress(user.Email, user.Email),
            "Confirm email and set you password",
                content,
            null);
            await _emailSender.SendEmailAsync(message);

            return Ok(user);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(string userId, UpdateUserDTO model)
        {

            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("User does not exists.");
            }

            user.FirstName = model.FirstName;
            user.FirstName = model.LastName;

            IdentityResult userUpdatedResult = await _userManager.UpdateAsync(user);


            if (userUpdatedResult.Succeeded)
            {
                IList<string> existingRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, existingRoles);
                await _userManager.AddToRolesAsync(user, model.Roles);
            }
            return Ok();
        }

        [HttpPut("{userId}/addtorole/{role}")]
        public async Task<ActionResult> AddToRole(string userId, string role)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("User does not exists.");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Role does not exists.");
            }

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return Ok();
        }

        [HttpDelete("{userId}/removefromrole/{role}")]
        public async Task<ActionResult> RemoveFromRole(string userId, string role)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("User does not exists.");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Role does not exists.");
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            return Ok();
        }


        [HttpPost("{userId}/disableuser")]
        public async Task<ActionResult> DisableUser(string userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("User does not exists.");
            }

            user.Enabled = false;

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(string userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return Ok();
        }

    }
}