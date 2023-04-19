using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Infrastructure.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Areas.Identity.Pages
{
    public class ProfileModel : PageModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public List<string> Roles { get; set; }

        public bool IsSuperAdmin { get; set; }
        private IMediator _mediatorInstance;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMediator mediatorInstance)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mediatorInstance = mediatorInstance;
        }

        public async Task OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserId = userId;
                Email = user.Email;
                Username = user.UserName;
                ProfilePicture = user.ProfilePicture;
                FullName = user.FullName;
                PhoneNumber = user.PhoneNumber;
                FirstName = user.FirstName;
                LastName = user.LastName;
                IsActive = user.IsActive;
                IsSuperAdmin = roles.Contains("SuperAdmin");
                Roles = roles.ToList();
            }
        }

        public async Task<IActionResult> OnPostActivateUserAsync(string userId)
        {
            if (User.IsInRole("SuperAdmin"))
            {
                var currentUser = await _userManager.FindByIdAsync(userId);
                currentUser.IsActive = true;
                //currentUser.ActivatedBy = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                await _userManager.UpdateAsync(currentUser);
                await OnGetAsync(userId);
                //  await _mediatorInstance.Send(new AddActivityLogCommand() { userId = userId, Action = "ActivateUser", NewValues = $"ActivateUser {currentUser.FullName}!." });
                return RedirectToPage("Profile", new { area = "Identity", userId = userId });
            }
            else return default;
        }

        public async Task<IActionResult> OnPostDeActivateUserAsync(string userId)
        {
            if (User.IsInRole("SuperAdmin"))
            {
                var currentUser = await _userManager.FindByIdAsync(userId);
                currentUser.IsActive = false;
                await _userManager.UpdateAsync(currentUser);
                // await _mediatorInstance.Send(new AddActivityLogCommand() { userId = userId, Action = "DeActivateUser", NewValues = $"DeActivateUser {currentUser.FullName}!." });
                return RedirectToPage("Profile", new { area = "Identity", userId = userId });
            }
            else return default;
        }
    }
}
