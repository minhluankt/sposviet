using Domain.Entities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public  interface IUserRepository
    {
        ResponseModel<CookieCustomerUser> LoginProvider(string LoginProvider, string ProviderKey,string email ="");
        Task<CookieCustomerUser> RegisterAsync(Customer model);
        Task<bool> UpdatePasswordAsync(string pass, int Id);
        Task<Customer> GetUserAsync(ClaimsPrincipal User);
        Task<Customer> GetFullUserAsync(ClaimsPrincipal User);
        Task<Customer> GetUserByIdCodeAsync(string code);
        Task<Customer> GetUserByUserNameAsync(string code);
        Task<ResponseModel<CookieCustomerUser>> Validate(LoginCustomerViewModel model);
        Task<bool> CheckPasswordCustomerAsync(string pass, int Id);
        Task<IList<Claim>> GetClaimsAsync(IKey userId);
        
    }
  

}
