using Application.Enums;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            //Seed Roles
            var role = new ApplicationRole(Roles.SuperAdmin.ToString());
            await roleManager.CreateAsync(role);
            //await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        }
    }
}