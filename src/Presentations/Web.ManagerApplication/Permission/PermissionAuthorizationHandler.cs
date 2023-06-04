using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using Domain.Identity;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Web.ManagerApplication.Permission
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ISignInManagerRepository<ApplicationUser> _signInManagerrRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IHttpContextAccessor _contextAccessor;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly IUserRepository _userrepository;
        public PermissionAuthorizationHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
            ISignInManagerRepository<ApplicationUser> signInManagerrRepository, IUserRepository userrepository,
            RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _userrepository = userrepository;
            _signInManagerrRepository = signInManagerrRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                context.Fail();
                return;
            }
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                context.Fail();
                return;
            }
            if (!user.IsActive || await _userManager.IsLockedOutAsync(user) || user.LockoutForever)
            {
                await _signInManager.SignOutAsync();
                await _signInManagerrRepository.SignOutAsync();
                return;
            }
            if (user.UserName.ToLower() == "superadmin" || user.Level == 2)
            {
                context.Succeed(requirement);
                return;
            }
            if (user.IdDichVu != EnumTypeProduct.NONE)
            {
                var routeData = _contextAccessor.HttpContext.GetRouteData();
                var areaName = routeData?.Values["area"]?.ToString();
                if (!string.IsNullOrEmpty(areaName) && areaName.ToLower().Equals("admin"))
                {
                    context.Fail();
                    return;
                }
            }
            
            if (user.IdDichVu == EnumTypeProduct.THOITRANG)
            {
                var routeData = _contextAccessor.HttpContext.GetRouteData();
                var areaName = routeData?.Values["area"]?.ToString();
                if (!string.IsNullOrEmpty(areaName) && areaName.ToLower().Equals("selling"))
                {
                    context.Fail();
                    return;
                }
            }



            var userRoleNames = await _userManager.GetRolesAsync(user);
            var userRoles = _roleManager.Roles.Where(x => userRoleNames.Contains(x.Name));
            var checkSupadmin = userRoles.Where(m => m.Name.ToLower().Contains("superadmin")).Count();
            if (checkSupadmin > 0)
            {
                context.Succeed(requirement);
                return;
            }
            foreach (var role in userRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                //var permissions = roleClaims.Where(x => x.Type == CustomClaimTypes.Permission &&
                //                                        x.Value == requirement.Permission &&
                //                                        x.Issuer == "LOCAL AUTHORITY")
                //                            .Select(x => x.Value);
                var permissions = roleClaims.Where(x => x.Type == CustomClaimTypes.Permission &&
                    x.Value == requirement.Permission)
                                          .Select(x => x.Value);
                if (permissions.Any())
                {
                    context.Succeed(requirement);
                    return;
                }
            }

        }
    }

}
