using AspNetCoreHero.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<bool> ValidatePhoneNumberAndEmail(string PhoneNumber, string Email, int ComId, int? Id);
        Task<string> ComplementaryEmailAsync(string Email,int ComId, int Id);
        Task SendMailConfirmAsync(Customer model,string titleemail="");
        Task<IResult<Customer>> Crreate(Customer model);
        Task<Customer> ConfirmEmailAccount(string Email, int Id);
        Task<bool> UpdatePhoneNumber(string phone, int Id);
        IEnumerable<Customer> GetAllIEnumerable(Customer model);
        bool ValidateEmail(string Email, int ComId, int? Id = null);
        Task<bool> ValidateCode(string Code, int? ComId);
        bool UpdateAvatar(string base64, int id, out string filename);
        bool ValidatePhoneNumber(string PhoneNumber, int ComId, int? Id = null);
        List<Customer> GetAllIQueryableDatatable(Customer model, string sortColumn, string sortColumnDirection, int pageSize, int skip, out int recordsTotal);
    }
}
