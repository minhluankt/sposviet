using Application.Constants;
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

namespace Application.Features.DefaultFoodOrders.Query
{
  
    public class GetAllDefaultFoodOrderQuery : IRequest<Result<IEnumerable<DefaultFoodOrder>>>
    {
        public int ComId { get; set; }
        public class GetAllDefaultFoodOrderHandler : IRequestHandler<GetAllDefaultFoodOrderQuery, Result<IEnumerable<DefaultFoodOrder>>>
        {
            private readonly IRepositoryAsync<DefaultFoodOrder> _repository;
            public GetAllDefaultFoodOrderHandler(IRepositoryAsync<DefaultFoodOrder> repository)
            {
                _repository = repository;
            }
            public async Task<Result<IEnumerable<DefaultFoodOrder>>> Handle(GetAllDefaultFoodOrderQuery query, CancellationToken cancellationToken)
            {
                var product =  _repository.Entities.AsNoTracking().Where(x=>x.ComId==query.ComId).AsEnumerable();
                return await Result<IEnumerable<DefaultFoodOrder>>.SuccessAsync(product);
            }
        }
    }
}
