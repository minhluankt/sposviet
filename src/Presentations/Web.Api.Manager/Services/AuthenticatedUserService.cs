using Application.Constants;
using Application.Interfaces.Shared;
using Infrastructure.Infrastructure.Migrations.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace Web.Api.Manager.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
            _comId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimUser.COMID)?.Value;
        }

        public string UserId { get; }
        public string Username { get; }
        public string _comId { get; }
        public int? ComId { get {
                if (!string.IsNullOrEmpty(_comId))
                {
                    return int.Parse(_comId);
                }
                return null;
            } 
        }
    }
}
