using Application.Constants;
using Domain.Identity;
using Domain.ViewModel;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Web.ManagerApplication.Areas.Admin.Models;

namespace Web.ManagerApplication.Helper
{
    //public static class ClaimsHelper
    //{
    //    public static void HasRequiredClaims(this ClaimsPrincipal claimsPrincipal, IEnumerable<string> permissions)
    //    {
    //        if (!claimsPrincipal.Identity.IsAuthenticated)
    //        {
    //            return;
    //        }
    //        var allClaims = claimsPrincipal.Claims.Select(a => a.Value).ToList();
    //        var success = allClaims.Intersect(permissions).Any();
    //        if (!success)
    //        {
    //            throw new Exception();
    //        }
    //        return;
    //    }
    //    public static void GetAllPermissions(this List<RoleClaimsViewModel> allPermissions, List<Domain.Entities.Permission> list)
    //    {
    //        // FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

    //        foreach (var fi in list)
    //        {
    //            allPermissions.Add(new RoleClaimsViewModel { Name = fi.Name, Value = fi.Code, Type = "Permissions" });
    //        }
    //    }
    //    //public static void GetPermissions(this List<RoleClaimsViewModel> allPermissions, Type policy, string roleId)
    //    //{
    //    //    FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

    //    //    foreach (FieldInfo fi in fields)
    //    //    {
    //    //        allPermissions.Add(new RoleClaimsViewModel { Value = fi.GetValue(null).ToString(), Type = "Permissions" });
    //    //    }
    //    //}

    //    public static async Task AddPermissionClaim(this RoleManager<ApplicationRole> roleManager, ApplicationRole role, string permission)
    //    {
    //        var allClaims = await roleManager.GetClaimsAsync(role);
    //        if (!allClaims.Any(a => a.Value == permission))
    //        {
    //            await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
    //        }
    //    }
    //}
}
