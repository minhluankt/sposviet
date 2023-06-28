
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ProductInBarAndKitchens.Query
{
    public class GetPaginatedListProductInBarAndKitchenQuery : DatatableModel, IRequest<Result<PaginatedList<ProductInBarAndKitchenModel>>>
    {
        public int ComId { get; set; }
        public int IdBarAndKitchen { get; set; }
        public int? IdCategory { get; set; }
        public string Name { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetPaginatedListProductInBarAndKitchenQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllProductInBarAndKitchendQueryHandler : IRequestHandler<GetPaginatedListProductInBarAndKitchenQuery, Result<PaginatedList<ProductInBarAndKitchenModel>>>
    {

        private readonly IProductInBarAndKitchenRepository _ProductInBarAndKitchen;
        private readonly IMapper _mapper;

        public GetAllProductInBarAndKitchendQueryHandler(IProductInBarAndKitchenRepository ProductInBarAndKitchen, IMapper mapper)
        {
            _ProductInBarAndKitchen = ProductInBarAndKitchen;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<ProductInBarAndKitchenModel>>> Handle(GetPaginatedListProductInBarAndKitchenQuery request, CancellationToken cancellationToken)
        {
            var productList = await _ProductInBarAndKitchen.GetPaginatedList(request.IdCategory, request.IdBarAndKitchen, request.Name, request.sortColumn, request.sortColumnDirection, request.PageNumber, request.PageSize, request.skip);
            return Result<PaginatedList<ProductInBarAndKitchenModel>>.Success(productList);
        }
    }
}
