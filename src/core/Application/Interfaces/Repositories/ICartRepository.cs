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
    public interface ICartRepository<T> where T : class
    {
        Task RemoveCart(int idCart);//0 là khách hàng
        IQueryable<T> GetCartCustomer(int id,int type = 0);//0 là khách hàng
        Task<int> GetQuantityCartByUserAsync(int IdCustomer);//0 là khách hàng
        Task<ResponseModel<OrderModelView>> CheckOutCart(Customer Customer);//0 là khách hàng
        Task<ResponseModel<CartModelView>> RemoveItemCartAsync(int IdCustomer,int IdItemCart);//0 là khách hàng
        Task<ResponseModel<CartModelView>> UpdateCartBySelectItemAsync(int idCus, int?[] IdItem, bool select,bool removeAll, bool checkAll);//0 là khách hàng
        Task<ResponseModel<CartModelView>> AddOrUpdateToCartAsync(bool AddToCart, Product Product, int quantity, int idCus, int typecustomer = 0);//0 là khách hàng
    }
}
