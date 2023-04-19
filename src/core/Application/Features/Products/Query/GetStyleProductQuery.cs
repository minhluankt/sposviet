using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Query
{

    public class GetStyleProductQuery : IRequest<Result<StyleAndOptionNameModelproduct>>
    {
        public int IdProduct { get; set; }

        public class GetProductByIdQueryHandler : IRequestHandler<GetStyleProductQuery, Result<StyleAndOptionNameModelproduct>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<StyleProduct> _repositoryStyleProduct;
            private readonly IRepositoryAsync<OptionsDetailtProduct> _repositoryOptionsDetailtProduct;
            private readonly IProductPepository<Product> _productrepository;
            public GetProductByIdQueryHandler(IRepositoryAsync<StyleProduct> repositoryStyleProduct,
                IMapper mapper,
                IRepositoryAsync<OptionsDetailtProduct> repositoryOptionsDetailtProduct
                , IProductPepository<Product> productrepository)
            {
                _mapper = mapper;
                _repositoryOptionsDetailtProduct = repositoryOptionsDetailtProduct;
                _productrepository = productrepository;
                _repositoryStyleProduct = repositoryStyleProduct;
            }
            public async Task<Result<StyleAndOptionNameModelproduct>> Handle(GetStyleProductQuery query, CancellationToken cancellationToken)
            {
                if (query.IdProduct == 0)
                {
                    return await Result<StyleAndOptionNameModelproduct>.FailAsync();
                }
                StyleAndOptionNameModelproduct model = new StyleAndOptionNameModelproduct();
                var styleproduct = _repositoryStyleProduct.Entities.Where(m => m.IdProduct == query.IdProduct).Include(x => x.OptionsNames).ToList();
                var mapstyleproduct = _mapper.Map<List<StyleProductModel>>(styleproduct);
                if (mapstyleproduct.Count() > 0 && mapstyleproduct[0].OptionsNames == null)
                {
                    model.styleProductModels = mapstyleproduct;
                    for (int i = 0; i < model.styleProductModels.Count(); i++)
                    {
                        model.styleProductModels[i].OptionsNames = _mapper.Map<List<OptionsNameProductModel>>(styleproduct[i].OptionsNames);
                    }
                }
                else
                {
                    model.styleProductModels = mapstyleproduct;
                }


                var styletableproduct = _repositoryOptionsDetailtProduct.Entities.Where(m => m.IdProduct == query.IdProduct).ToList();
                var mapOptionsDetailtProductModels = _mapper.Map<List<OptionsDetailtProductModel>>(styletableproduct);
                model.OptionsDetailtProductModels = mapOptionsDetailtProductModels;

                return await Result<StyleAndOptionNameModelproduct>.SuccessAsync(model);
            }
        }
    }
}
