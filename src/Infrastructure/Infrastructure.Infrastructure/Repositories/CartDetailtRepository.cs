using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class CartDetailtRepository : ICartDetailtRepository
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<CartDetailt> _repositoryCartitem;
        public CartDetailtRepository(IUnitOfWork unitOfWork, IRepositoryAsync<CartDetailt> repositoryCart)
        {
            _unitOfWork = unitOfWork;
            _repositoryCartitem = repositoryCart;
        }

        public void AddItem(CartDetailt cartitem, bool commit = false)
        {
            _repositoryCartitem.AddAsync(cartitem);
            if (commit)
            {
                _unitOfWork.SaveChangesAsync();
            }
        }

        public bool CheckProduct(int idCart, int idproduct, out int idCartItem)
        {
            var getitem = _repositoryCartitem.Entities.SingleOrDefault(x => x.IdProduct == idproduct && x.IdCart == idCart);
            if (getitem == null)
            {
                idCartItem = 0;
                return false;
            }
            idCartItem = getitem.Id;
            return true;
        }

        public async void UpdateItem(CartDetailt cartitem, bool addQuantity, bool commit = false)
        {
            // có 2 laoij addQuantity nếu đătk từ trang chủ hoạc chi tiết là cộng còn ở giỏ thì lấy luôn giá trịd
            var getcat = _repositoryCartitem.GetById(cartitem.Id);
            if (getcat != null)
            {
                if (addQuantity)
                {

                    getcat.Quantity = getcat.Quantity + cartitem.Quantity;
                }
                else
                {
                    getcat.Quantity = cartitem.Quantity;
                }

                getcat.Discount = cartitem.Discount;
                if (!string.IsNullOrEmpty(cartitem.Name))
                {
                    getcat.Name = cartitem.Name;
                }
                // getcat.isSelected = cartitem.isSelected;
                await _repositoryCartitem.UpdateAsync(getcat);
            }
            if (commit)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public int GetQuantityByCart(int idCart)
        {
            return _repositoryCartitem.Entities.Where(x => x.IdCart == idCart && x.isSelected).Sum(x => x.Quantity);
        }
        public decimal GetAmountByCart(int idCart)
        {
            var getCart = _repositoryCartitem.Entities.Where(x => x.IdCart == idCart && x.isSelected).Include(x => x.Product).ToList();
            decimal Amount = 0;
            foreach (var item in getCart)
            {
                decimal price = item.Product.Price;
                if (item.Product.isRunPromotion)
                {
                    if (item.Product.PriceDiscountRun > 0)
                    {
                        price = item.Product.PriceDiscountRun;
                    }
                    else if (item.Product.DiscountRun > 0)
                    {
                        price = item.Product.Price - ((decimal)(item.Product.DiscountRun / 100) * item.Product.Price);
                    }
                }
                else if (item.Product.isPromotion && item.Product.ExpirationDateDiscount != null)
                {
                    if (item.Product.ExpirationDateDiscount.Value >= DateTime.Now)
                    {   // tức là còn hạn mới áp dụng
                        if (item.Product.PriceDiscount > 0)
                        {
                            price = item.Product.PriceDiscount;
                        }
                        else if (item.Product.Discount > 0)
                        {
                            price = item.Product.Price - ((decimal)(item.Product.Discount / 100) * item.Product.Price);
                        }
                    }
                }
                Amount += item.Quantity * price;
            }
            //return _repositoryCartitem.Entities.Where(x => x.IdCart == idCart && x.isSelected).Include(x => x.Product).Sum(x => x.Quantity * (x.Product != null ? x.Product.Price : 0));
            return Amount;
        }

        public List<CartDetailt> GetListItemByCart(int idCart)
        {
            return _repositoryCartitem.GetAll(x => x.IdCart == idCart).ToList();
        }

        public void UpdateSelectItem(int IdCart, int IdItem, bool select, out decimal Amount, out int Quantity)
        {
            var getcat = _repositoryCartitem.Entities.Where(x => x.IdCart == IdCart && x.Id == IdItem).Include(x => x.Product).SingleOrDefault();
            if (getcat != null)
            {
                getcat.isSelected = select;
                Amount = getcat.Quantity * getcat.Product.Price;
                Quantity = getcat.Quantity;
                _repositoryCartitem.Update(getcat);
            }
            else
            {
                Amount = 0;
                Quantity = 0;
            }
        }

        public void UpdateDisableAllItem(int IdCart, out decimal Amount, out int Quantity)
        {
            var getcat = _repositoryCartitem.Entities.Where(x => x.IdCart == IdCart).Include(x => x.Product).ToList();
            if (getcat.Count() > 0)
            {
                getcat.ForEach(x => x.isSelected = false);
                // Amount = getcat.Where(x=>x.isSelected).Sum(x=>x.Quantity*(x.Product!=null? x.Product.Price:0));
                //  Quantity = getcat.Where(x => x.isSelected).Sum(x=>x.Quantity);
                Amount = 0;
                Quantity = 0;
                _repositoryCartitem.UpdateRange(getcat);
            }
            else
            {
                Amount = 0;
                Quantity = 0;
            }
        }

        public void RemoveItemCart(int IdCart, int IdItemCart, out decimal Amount, out int Quantity)
        {
            var getcat = _repositoryCartitem.Entities.Where(x => x.IdCart == IdCart && x.Id == IdItemCart).Include(x => x.Product).SingleOrDefault();
            if (getcat != null)
            {
                Amount = getcat.Quantity * getcat.Product.Price;
                Quantity = getcat.Quantity;
                _repositoryCartitem.Delete(getcat);
            }
            else
            {
                Amount = 0;
                Quantity = 0;
            }

        }

        public async Task RemoveListItemCart(int IdCart, List<int> lstItemCart)
        {
            if (lstItemCart.Count() > 0)
            {
                var getcat = await _repositoryCartitem.Entities.Where(x => x.IdCart == IdCart && lstItemCart.ToArray().Contains(x.Id)).ToListAsync();
                await _repositoryCartitem.DeleteRangeAsync(getcat);
            }
        }
    }
}
