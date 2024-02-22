using Applogiq.IdentityServer.Constants;
using Applogiq.IdentityServer.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace Applogiq.IdentityServer.DataSeed
{
    public static class IdentityServerDataInitializer
    {
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.SeedRolesAsync();
            await userManager.SeedUsersAsync();
        }

        private static async Task SeedUsersAsync(this UserManager<ApplicationUser> userManager)
        {

            await CreateUserAsync(userManager, "Super Admin", "superadmin@Applogiq", RoleConstant.SuperAdminRoleName);

            await CreateUserAsync(userManager, "Member User", "member@Applogiq", RoleConstant.MemberRoleName);
        }

        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, string name, string email, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                ApplicationUser user = new()
                {
                    FirstName = name.Split(" ")[0],
                    LastName = name.Split(" ")[1],
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Enabled = true
                };

                IdentityResult result = await userManager.CreateAsync(user, "Applogiq@software0211");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            else
            {
                ApplicationUser? user = await userManager.FindByEmailAsync(email);
                bool isUserAnAdmin = await userManager.IsInRoleAsync(user, role);
                if (!isUserAnAdmin)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private static async Task CreateRoleAsync(this RoleManager<IdentityRole> roleManager,
               string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                IdentityRole role = new()
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };

                await roleManager.CreateAsync(role);
            }
        }



        private static async Task SeedRolesAsync(this RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateRoleAsync(RoleConstant.SuperAdminRoleName);
            await roleManager.CreateRoleAsync(RoleConstant.MemberRoleName);
        }

    }
}