using Applogiq.IdentityServer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Applogiq.IdentityServer.Controllers
{
    [Authorize(Roles = RoleConstant.SuperAdminRoleName)]
    public class RolesController : DefaultBaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(
            RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<IdentityRole>> GetRolesById(string roleId)
        {
            IdentityRole? role = await _roleManager
                .Roles
                .Where(x => x.Id == roleId)
                .FirstOrDefaultAsync();

            return role == null ? (ActionResult<IdentityRole>)NotFound("Role not found") : (ActionResult<IdentityRole>)Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult> AddRole(IdentityRole model)
        {
            await _roleManager.CreateAsync(model);
            return Ok();
        }

        [HttpPut("{roleId}")]
        public async Task<ActionResult> UpdateRole(string roleId, IdentityRole model)
        {
            IdentityRole? role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return BadRequest("Role does not exists.");
            }
            role.Name = model.Name;
            await _roleManager.UpdateAsync(role);

            return Ok();
        }

        [HttpDelete("{roleId}")]
        public async Task<ActionResult> Delete(string roleId)
        {
            IdentityRole? role = await _roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return Ok();
        }

        [HttpGet("{roleId}/claims")]
        public async Task<ActionResult> GetClaims(string roleId)
        {
            IdentityRole? _role = await _roleManager.FindByIdAsync(roleId);

            IList<Claim> claims = await _roleManager.GetClaimsAsync(_role);

            return Ok(claims);
        }

        [HttpPost("{roleId}/claims")]
        public async Task<ActionResult> UpdateClaims(string roleId, IEnumerable<string> permissions)
        {
            IdentityRole? role = await _roleManager.FindByIdAsync(roleId);

            IList<Claim> existingClaims = await _roleManager.GetClaimsAsync(role);

            foreach (Claim claim in existingClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (string permission in permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim(permission, permission));
            }

            return Ok();
        }
    }
}