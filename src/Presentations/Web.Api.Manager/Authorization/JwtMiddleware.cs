using Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Web.Api.Manager.Authorization
{
    public class JwtMiddleware
    {
        //private readonly IIdentityService _identityService;
        //private readonly RequestDelegate _next;

        //public JwtMiddleware(RequestDelegate next, IIdentityService identityService)
        //{
        //    _next = next;
        //    this._identityService = identityService;
        //}

        //public async Task Invoke(HttpContext context, IUserService userService, IIdentityService jwtUtils)
        //{
        //    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //    var userId = jwtUtils.ValidateToken(token);
        //    if (userId != null)
        //    {
        //        // attach user to context on successful jwt validation
        //        context.Items["User"] = userService.GetById(userId.Value);
        //    }

        //    await _next(context);
        //}
    }
}
