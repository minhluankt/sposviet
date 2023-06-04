using Application.Constants;
using Application.DTOs.Identity;
using Application.DTOs.Mail;
using Application.DTOs.Settings;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Application.Providers;
using AspNetCoreHero.Results;
using AspNetCoreHero.ThrowR;
using Domain.Entities;
using Domain.Identity;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserManager _customerAuthenManager;
        private readonly IUserRepository _customerManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMailService _mailService;
        private readonly ILogger<IdentityService> _logger;
        public IdentityService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, ILogger<IdentityService> logger,
            IOptions<JWTSettings> jwtSettings,
            IUserRepository customerManager, IUserManager customerAuthenManager,
           TokenValidationParameters tokenValidationParameters,
            IDateTimeService dateTimeService,
            SignInManager<ApplicationUser> signInManager, IMailService mailService)
        {
            _logger = logger;
            _customerManager = customerManager;
            _customerAuthenManager = customerAuthenManager;
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _dateTimeService = dateTimeService;
            _signInManager = signInManager;
            _mailService = mailService;
        }

        //public async Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress)
        //{
        //    var user = await _userManager.FindByNameAsync(request.Email);

        //    Throw.Exception.IfNull(user, nameof(user), $"No Accounts Registered with {request.Email}.");
        //    var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        //   //Throw.Exception.IfFalse(user.EmailConfirmed, $"Email is not confirmed for '{request.Email}'.");
        //    Throw.Exception.IfFalse(user.IsActive, $"Account for '{request.Email}' is not active. Please contact the Administrator.");
        //    Throw.Exception.IfFalse(result.Succeeded, $"Invalid Credentials for '{request.Email}'.");
        //    JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
        //    var response = new TokenResponse();
        //    response.Id = user.Id;
        //    response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        //    response.IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime();
        //    response.ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
        //    response.Email = user.Email;
        //    response.UserName = user.UserName;
        //    var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        //    response.Roles = rolesList.ToList();
        //    response.IsVerified = user.EmailConfirmed;
        //    var refreshToken = GenerateRefreshToken(ipAddress);
        //    response.RefreshToken = refreshToken.Token;
        //    return Result<TokenResponse>.Success(response, "Authenticated");
        //}
        public async Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress)
        {
           // var user = await _customerManager.GetUserByUserNameAsync(request.Email);//khách hàng customer
            var user = await _userManager.FindByNameAsync(request.Email);
            if (request.isOwner)
            {
                if (!user.IsStoreOwner)
                {
                    return Result<TokenResponse>.Fail($"Tài khoản không phải là Owner");
                }
            }
            //Throw.Exception.IfNull(user, nameof(user), $"Tài khoản chưa đăng ký {request.Email}.");
            if (user==null)
            {
                return Result<TokenResponse>.Fail($"Tài khoản chưa đăng ký {request.Email}");
            }
            //var result = _customerAuthenManager.PasswordSignIn(user, request.Password, false);//khách hàng
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            //Throw.Exception.IfFalse(user.EmailConfirmed, $"Email is not confirmed for '{request.Email}'.");
            // Throw.Exception.IfFalse(user.Status == (int)CustomerAccountStatus.Lock, $"Account for '{request.Email}' is not active. Please contact the Administrator.");
            // Throw.Exception.IfFalse(result.Succeeded, $"Invalid Credentials for '{request.Email}'.");
            Throw.Exception.IfFalse(result.Succeeded, $"Tên đăng nhập hoặc mật khẩu không hợp lệ");
           
            if (result.RequiresTwoFactor)
            {
               // return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("App User account locked out." + request.Email);
                Throw.Exception.IfFalse(result.Succeeded, $"Tài khoản đã bị khóa");
            }
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            var response = new TokenResponse();
            response.Id = user.Id;
            response.ComId = user.ComId;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime();
            response.ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
            response.Email = user.Email;
            response.UserName = user.UserName;
            // var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            // response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return Result<TokenResponse>.Success(response, "Đăng nhập thành công");
        }
        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            try
            {
                tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "uid").Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
        public async Task<Result<TokenResponse>> RefreshToken(RefreshTokenModel request, string ipAddress)
        {
            string? accessToken = request.AccessToken;
            //  string? refreshToken = request.RefreshToken;
            var principal = GetPrincipalFromExpiredToken(accessToken);

            if (principal == null)
            {
                Throw.Exception.IfFalse(false, $"Invalid Credentials for");
            }
            string username = principal.Identity.Name;
           // var user = await _customerManager.GetUserByUserNameAsync(username);//khách ahfng
            var user = await _userManager.FindByNameAsync(username);
            Throw.Exception.IfNull(user, nameof(user), $"Tài khoản không tồn tại {user.UserName}.");
            Throw.Exception.IfFalse(user.LockoutEnabled, $"Tài khoản '{user.UserName}' chưa được kích hoạt. Liên hệ admin để hỗ trợ.");

            JwtSecurityToken jwtSecurityToken = await  GenerateJWToken(user, ipAddress);
            var response = new TokenResponse();
            response.Id = user.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime();
            response.ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
            response.Email = user.Email;
            response.UserName = user.UserName;
            //var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            // response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return Result<TokenResponse>.Success(response, "Authenticated");
        }
        //public async Task<Result<TokenResponse>> RefreshToken(RefreshTokenModel request, string ipAddress)
        //{
        //    string? accessToken = request.AccessToken;
        //    //  string? refreshToken = request.RefreshToken;
        //    var principal = GetPrincipalFromExpiredToken(accessToken);

        //    if (principal == null)
        //    {
        //        Throw.Exception.IfFalse(false, $"Invalid Credentials for");
        //    }
        //    string username = principal.Identity.Name;
        //    var user = await _userManager.FindByNameAsync(username);
        //    Throw.Exception.IfNull(user, nameof(user), $"No Accounts Registered with {user.UserName}.");
        //    Throw.Exception.IfFalse(user.IsActive, $"Account for '{user.UserName}' is not active. Please contact the Administrator.");

        //    JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
        //    var response = new TokenResponse();
        //    response.Id = user.Id;
        //    response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        //    response.IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime();
        //    response.ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
        //    response.Email = user.Email;
        //    response.UserName = user.UserName;
        //    var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        //    response.Roles = rolesList.ToList();
        //    response.IsVerified = user.EmailConfirmed;
        //    var refreshToken = GenerateRefreshToken(ipAddress);
        //    response.RefreshToken = refreshToken.Token;
        //    return Result<TokenResponse>.Success(response, "Authenticated");
        //}
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
        private JwtSecurityToken GenerateJWToken(Customer user, string ipAddress)
        {
            try
            {
                // var userClaims = await _customerManager.GetClaimsAsync(user);
                // var roles = await _customerManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();
                //for (int i = 0; i < roles.Count; i++)
                //{
                //    roleClaims.Add(new Claim("roles", roles[i]));
                //}
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.IdCodeGuid.ToString()),
               // new Claim("first_name", user.FirstName),
                //new Claim("UserName", user.UserName),
                new Claim("fullname", $"{user.Name}"),
                new Claim("ip", ipAddress)
            }
                // .Union(userClaims)
                .Union(roleClaims);
                return JWTGeneration(claims);
            }
            catch (Exception e)
            {
                throw;
            }

        }
        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user, string ipAddress)
        {
            try
            {
                //var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();
                for (int i = 0; i < roles.Count; i++)
                {
                    roleClaims.Add(new Claim("roles", roles[i]));
                }
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               // new Claim(JwtRegisteredClaimNames.Email, user.Email),

                new Claim(ClaimUser.FULLNAME, user.FullName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimUser.IDGUID, user.Id),
                new Claim(ClaimUser.COMID, user.ComId.ToString()),
                new Claim(ClaimUser.IDDICHVU, ((int)user.IdDichVu).ToString()),

                new Claim("ip", ipAddress)
                }
               // .Union(userClaims)
                .Union(roleClaims);
                return JWTGeneration(claims);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        private JwtSecurityToken JWTGeneration(IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(1),
                Created = DateTime.Now,
                CreatedByIp = ipAddress
            };
        }

        public async Task<Result<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new Exception($"Username '{request.UserName}' is already taken.");
            }
            var user = new ApplicationUser
            {
                Email = request.Email,
                FullName = request.FullName,
                //ad = request.Address,
                UserName = request.UserName
            };
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, Roles.Basic.ToString()); add quyền mặc định
                    var verificationUri = await SendVerificationEmail(user, origin);
                    //TODO: Attach Email Service here and configure it via appsettings
                    List<string> list = new List<string>();
                    list.Add(user.Email);
                    string[] listmail = list.ToArray();

                    var builder = new BodyBuilder();
                    builder.HtmlBody = $"Please confirm your account by <a href='{verificationUri}'>clicking here</a>.";
                    MimeEntity aContent = builder.ToMessageBody();

                    MailRequest mailRequest = new MailRequest
                    {
                        To = listmail,
                        Title = "Confirm your email",
                        Subject = "Confirm Registration",
                        Body = aContent
                    };
                    await _mailService.SendAsync(mailRequest);
                    return Result<string>.Success(user.Id, message: $"User Registered. Confirmation Mail has been delivered to your Mailbox. (DEV) Please confirm your account by visiting this URL {verificationUri}");
                }
                else
                {
                    throw new Exception($"{result.Errors}");
                }
            }
            else
            {
                throw new Exception($"Email {request.Email } is already registered.");
            }
        }

        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/identity/confirm-email/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }

        public async Task<Result<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Result<string>.Success(user.Id, message: $"Account Confirmed for {user.Email}. You can now use the /api/identity/token endpoint to generate JWT.");
            }
            else
            {
                throw new Exception($"An error occured while confirming {user.Email}.");
            }
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            var route = "api/identity/reset-password/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));

            List<string> list = new List<string>();
            list.Add(model.Email);
            string[] listmail = list.ToArray();

            var builder = new BodyBuilder();
            builder.HtmlBody = $"You reset token is - {code}";
            MimeEntity aContent = builder.ToMessageBody();

            MailRequest mailRequest = new MailRequest
            {
                To = listmail,
                Title = "Reset Password",
                Subject = "Reset Password",
                Body = aContent
            };

            await _mailService.SendAsync(mailRequest);
        }

        public async Task<Result<string>> ResetPassword(ResetPasswordRequest model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null) throw new Exception($"No Accounts Registered with {model.Email}.");
            var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);
            if (result.Succeeded)
            {
                return Result<string>.Success(model.Email, message: $"Password Resetted.");
            }
            else
            {
                throw new Exception($"Error occured while reseting the password.");
            }
        }
    }
}
