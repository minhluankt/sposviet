using Application.Constants;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Views.Shared.Components.ProfileSlidebar
{
    public class ProfileSlidebarViewComponent : ViewComponent
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IHttpContextAccessor _contextAccessor;
        private HttpContext _context;
        private readonly IUserRepository _userRepository;
        public ProfileSlidebarViewComponent(IUserRepository userRepository
            , IFormFileHelperRepository fileHelper,
            IHttpContextAccessor contextAccessor)
        {
            _fileHelper = fileHelper;
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
        }
        public HttpContext Context
        {
            get
            {
                var context = _context ?? _contextAccessor?.HttpContext;
                if (context == null)
                {
                    throw new InvalidOperationException("HttpContext must not be null.");
                }
                return context;
            }
            set
            {
                _context = value;
            }
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userRepository.GetUserAsync(Context.User);
            if (!string.IsNullOrEmpty(user.Image))
            {
                user.Image = _fileHelper.ImagetoBase64(user.Image, FolderUploadConstants.Customer);
            }
            return View(user);
        }
    }
}
