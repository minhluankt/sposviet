using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Customers.Query
{

    public class GetByCartCustomerQuery : IRequest<Result<CustomerModelView>>
    {
        public int IdCustomer { get; set; }
        public bool IsSelectCart { get; set; }
        public GetByCartCustomerQuery(int id)
        {
            IdCustomer = id;
        }
        public class GetCustomerByIdQueryHandler : IRequestHandler<GetByCartCustomerQuery, Result<CustomerModelView>>
        {
            private IOptions<CryptoEngine.Secrets> _config;
            private readonly IRepositoryAsync<Customer> _repository;
            private readonly ICartRepository<Cart> _repositorycart;

            public GetCustomerByIdQueryHandler(IRepositoryAsync<Customer> repository,
                   IOptions<CryptoEngine.Secrets> config,
                ICartRepository<Cart> repositorycart)
            {
                _config = config;
                _repositorycart = repositorycart;
                _repository = repository;
            }
            public async Task<Result<CustomerModelView>> Handle(GetByCartCustomerQuery request, CancellationToken cancellationToken)
            {
                CustomerModelView customerModelView = new CustomerModelView();
                var datalist = await _repositorycart.GetCartCustomer(request.IdCustomer).Include(m => m.CartDetailts.OrderByDescending(m => m.Id)).ThenInclude(x => x.Product).SingleOrDefaultAsync();

                if (datalist != null)
                {
                    customerModelView.IdCart = datalist.Id;

                    CartModel cartModel = new CartModel();
                    cartModel.Id = datalist.Id;
                    cartModel.Amount = datalist.Amount;
                    cartModel.Total = datalist.Total;
                    cartModel.Quantity = datalist.Quantity;
                    customerModelView.CartModel = cartModel;

                    var listcart = datalist.CartDetailts.Select(x => new
                         CartDetailtModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Img = (x.Product != null ? x.Product.Img : ""),
                        Amount = x.Quantity * (x.Product != null ? x.Product.Price : 0),
                        Price = (x.Product != null ? x.Product.Price : 0),
                        PriceDiscountRun = (x.Product != null ? x.Product.PriceDiscountRun : 0),
                        PriceDiscount = (x.Product != null ? x.Product.PriceDiscount : 0),
                        Discount = (x.Product != null ? x.Product.Discount : 0),
                        DiscountRun = (x.Product != null ? x.Product.DiscountRun : 0),
                        IdProduct = (x.Product != null ? x.Product.Id : 0),
                        isPromotion = (x.Product != null ? x.Product.isPromotion : false),
                        isPromotionRun = (x.Product != null ? x.Product.isRunPromotion : false),
                        ExpirationDateDiscount = (x.Product != null ? x.Product.ExpirationDateDiscount : null),
                        Quantity = x.Quantity,
                        IdCart = x.IdCart,
                        isSelected = x.isSelected,
                        isDisable = x.Product != null ? x.Product.IsOutstock : false, // hết hàng
                    });

                    if (request.IsSelectCart)
                    {
                        listcart = listcart.Where(x => x.isSelected);
                    }

                    customerModelView.CartDetailts = listcart.ToList();
                    foreach (var item in customerModelView.CartDetailts)
                    {
                        decimal price = item.Price;


                        if (item.isPromotionRun)
                        {
                            if (item.PriceDiscountRun > 0)
                            {
                                price = item.PriceDiscountRun;
                            }
                            else if (item.DiscountRun > 0)
                            {
                                price = item.Price - ((decimal)(item.DiscountRun / 100) * item.Price);
                            }
                        }
                        else if (item.isPromotion && item.ExpirationDateDiscount != null)
                        {
                            if (item.ExpirationDateDiscount.Value >= DateTime.Now)
                            {   // tức là còn hạn mới áp dụng
                                if (item.PriceDiscount > 0)
                                {
                                    price = item.PriceDiscount;
                                }
                                else if (item.Discount > 0)
                                {
                                    price = item.Price - ((decimal)(item.Discount / 100) * item.Price);
                                }
                            }

                        }
                        item.Amount = item.Quantity * price;
                    }
                    //foreach (var item in customerModelView.CartDetailts)
                    //{
                    //    var values = "id=" + item.Id;
                    //    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    //    item.secretId = secret;
                    //}
                }
                else
                {
                    customerModelView.CartDetailts = new List<CartDetailtModel>();
                }

                return await Result<CustomerModelView>.SuccessAsync(customerModelView);
            }
        }
    }
}
