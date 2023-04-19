using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ICartDetailtRepository
    {
        List<CartDetailt> GetListItemByCart(int idCart);
        decimal GetAmountByCart(int idCart);
        int GetQuantityByCart(int idCart);
        bool CheckProduct(int idCart, int idproduct, out int idCartItem);
        void AddItem(CartDetailt cartitem,bool commit = false);
        void UpdateItem(CartDetailt cartitem, bool addQuantity, bool commit = false);
        void UpdateSelectItem(int IdCart,int IdItem, bool select, out decimal Amount, out int Quantity);
        void UpdateDisableAllItem(int IdCart,out decimal Amount, out int Quantity);
        void RemoveItemCart(int IdCart, int IdItemCart, out decimal Amount, out int Quantity);
        Task RemoveListItemCart(int IdCart, List<int> IdItemCart);
    }
}
