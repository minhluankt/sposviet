using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Query
{
    public class SearchProductQuery : IRequest<Result<IQueryable<Product>>>
    {
        public int ComId { get; set; }
        public EnumTypeProduct typeProduct { get; set; } = EnumTypeProduct.NONE;
        public int? idCity { get; set; }
        public string text { get; set; }
        public string code { get; set; }
        public bool isRunPromotion { get; set; }//sản phẩm sell  chạy ct theo giờ
        public bool isPromotion { get; set; }//sản phẩm sell k chạy ct
        public int IdPromotionRun { get; set; }
        public int Idcate { get; set; }
        public int[] LstIdProduct { get; set; }
        public bool IncludeCategory { get; set; }
        public bool IsTolist { get; set; }
        public bool CheckIsStopBusiness { get; set; }
        public bool CheckExpirationDateDiscount { get; set; }

    }
    public class SearchProductQueryHandler : IRequestHandler<SearchProductQuery, Result<IQueryable<Product>>>
    {

        private readonly IRepositoryAsync<Product> _ProductRepository;
        private readonly IProductPepository<Product> _Product;
        private readonly IMapper _mapper;

        public SearchProductQueryHandler(IRepositoryAsync<Product> ProductRepository, IMapper mapper, IProductPepository<Product> Product)
        {
            _Product = Product;
            _ProductRepository = ProductRepository;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<Product>>> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Product> productList = _ProductRepository.GetAllQueryable().AsNoTracking();
            if (request.typeProduct != EnumTypeProduct.NONE)
            {
                productList = productList.Where(x => x.TypeProduct == request.typeProduct);
            }
            if (request.ComId > 0)
            {
                productList = productList.Where(x => x.ComId == request.ComId);
            }
            if (!string.IsNullOrEmpty(request.text))
            {
                if (!string.IsNullOrEmpty(request.code))
                {
                    productList = productList.Where(m => m.Name.ToLower().Contains(request.text.ToLower()) || m.Code.ToLower().Contains(request.code.ToLower()));
                }
                else
                {
                    productList = productList.Where(m => m.Name.ToLower().Contains(request.text.ToLower()));
                }
            }


            if (request.isRunPromotion)
            {
                productList = productList.Where(m => m.isRunPromotion);
            }
            if (request.isPromotion)
            {
                productList = productList.Where(m => m.isPromotion && !m.isRunPromotion);
            }
            if (request.IdPromotionRun > 0)
            {
                productList = productList.Where(m => m.IdPromotionRun == request.IdPromotionRun);
            }
            if (request.CheckExpirationDateDiscount)
            {
                productList = productList.Where(m => m.ExpirationDateDiscount >= System.DateTime.Now);
            } 
            if (request.CheckIsStopBusiness)//tìm cái k ngưng
            {
                productList = productList.Where(m => !m.StopBusiness);
            }
            if (request.LstIdProduct != null)
            {
                productList = productList.Where(x=> request.LstIdProduct.Contains(x.Id));
            }///json bảng
            if (request.IncludeCategory)
            {
                productList = productList.Include(s => s.CategoryProduct);
            }
           
            if (request.IsTolist)
            {
                return await Result<IQueryable<Product>>.SuccessAsync(productList.ToList().AsQueryable());
            }
            return await Result<IQueryable<Product>>.SuccessAsync(productList);
        }
    }
}
