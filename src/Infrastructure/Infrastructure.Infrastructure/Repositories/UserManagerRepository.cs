using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Identity;
using Domain.Identity;

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Infrastructure.Infrastructure.Repositories
{
    public class UserManagerRepository : IUserManagerRepository<ApplicationUser>
    {
        private readonly IDapperRepository _Dapper;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserManagerCacheRepository<ApplicationUser> _usercacheManager;
        public UserManagerRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IDapperRepository Dapper,
            IUserManagerCacheRepository<ApplicationUser> usercacheManager)
        {
            _Dapper = Dapper;
            _roleManager = roleManager;
            _userManager = userManager;
            _usercacheManager = usercacheManager;
        }
        public List<string> GetRoles(ApplicationUser user)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("UserId", user.Id);
            string sql = "select * from UserRoles where UserId = @UserId";
            var _userRoles =  _Dapper.GetAll<IdentityUserRole<string>>(sql, param);
           
         
            return _userRoles.Select(x=>x.RoleId).ToList();
        }
        public  bool IsInRoleAsync(ApplicationUser user, string rolevalue)
        {
            //var rolesusess =  _roleManager.Roles.Where(x => x.ComId == user.ComId).ToList();
            var rolesuse = this.GetRoles(user);
            var roles =  _roleManager.Roles.Where(x=> rolesuse.Contains(x.Id)).Select(x=>x.Code).ToList();
            if ( roles.Any(x=>x== rolevalue))
            {
                return true;
            }
            // return  await _roleManager.Roles.Where(x => x.ComId == user.ComId).AnyAsync(x=>x.Code== rolevalue);
            return false;
        }
        public ApplicationUser GetDataUser(string userId)
        {
            var lst = _usercacheManager.GetListDataUserCache();
            return lst.SingleOrDefault(m => m.Id == userId);
        }
        public async Task<List<ApplicationUser>> GetAllUserAsync(ClaimsPrincipal principal)
        {
            var currentUser = await _userManager.GetUserAsync(principal);
            var allUsersExceptCurrentUser =  _userManager.Users.Where(x => x.ComId == currentUser.ComId&&!x.IsStoreOwner).ToList();
            return allUsersExceptCurrentUser;
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal, string schema = "")
        {
            if (!string.IsNullOrEmpty(schema))
            {
                var getname = principal.Identities.Where(m => m.AuthenticationType == schema).SingleOrDefault();
                if (getname != null)
                {
                    return await _userManager.FindByNameAsync(getname.Name);
                    //return await _userManager.FindByNameAsync(getname.Issuer);
                }
            }
            else
            {
                var getname = principal.Identities.FirstOrDefault();
                if (getname != null)
                {
                    return await _userManager.FindByNameAsync(getname.Name);
                    //return await _userManager.FindByNameAsync(getname.Issuer);
                }
            }
            return null;
        }
    }
}
