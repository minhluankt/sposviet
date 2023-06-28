using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ProductInBarAndKitchens.Query
{

    public class GetAllProductInBarAndKitchenQuery : IRequest<Result<IQueryable<ProductInBarAndKitchen>>>
    {
        public int ComId { get; set; }
        public int IdBar { get; set; }
        public class GetAllProductInBarAndKitchenHandler : IRequestHandler<GetAllProductInBarAndKitchenQuery, Result<IQueryable<ProductInBarAndKitchen>>>
        {
            private readonly IRepositoryAsync<ProductInBarAndKitchen> _repository;
            public GetAllProductInBarAndKitchenHandler(IRepositoryAsync<ProductInBarAndKitchen> repository)
            {
                _repository = repository;
            }
            public async Task<Result<IQueryable<ProductInBarAndKitchen>>> Handle(GetAllProductInBarAndKitchenQuery query, CancellationToken cancellationToken)
            {
                var product = _repository.Entities.AsNoTracking().Where(x => x.IdBarAndKitchen == query.IdBar);
                return await Result<IQueryable<ProductInBarAndKitchen>>.SuccessAsync(product);
            }
        }
    }
}
