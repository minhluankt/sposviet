using Application.Enums;
using Application.Hepers;
using Domain.Entities;
using Domain.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IOrderRepository<T> where T : class
    {
       // Task<T> GetbyCode(string code);
        IQueryable<T> GetOrderCustomer(int id, int type = 0);//0 là khách hàng
        IQueryable<T> GetAllOrder();//0 là khách hàng
        Task<PaginatedList<Order>> GetAllOrderAsync(OrderViewModel model);//0 là khách hàng
        void AddOrder(Order order);//0 là khách hàng
        Task<T> GetByIdOrderAndCustomerAsync(string codeOrder, int idCustomer);
        Task<T> GetOrderByIdAsync(int id);
        Task<ResponseModel<StatusOrder>> UpdateStatusAsync(int idOrder,EnumStatusOrder status, string note, string updateby, bool isCustomer=false);
        Task<ResponseModel<string>> DeleteByIdAsync(int id);
        Task<ResponseModel<StatusOrder>> CancelByIdAsync(int id,string note, string updateby,bool isCustomer= true);
    }
}
