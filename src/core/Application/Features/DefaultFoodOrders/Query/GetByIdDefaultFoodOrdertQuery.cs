using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.DefaultFoodOrders.Query
{

    public class GetByIdDefaultFoodOrderQuery : IRequest<Result<DefaultFoodOrder>>
    {
        public int Id { get; set; }

        public class GetDefaultFoodOrderByIdQueryHandler : IRequestHandler<GetByIdDefaultFoodOrderQuery, Result<DefaultFoodOrder>>
        {
            private readonly IRepositoryAsync<DefaultFoodOrder> _repository;
            public GetDefaultFoodOrderByIdQueryHandler(IRepositoryAsync<DefaultFoodOrder> repository)
            {
                _repository = repository;
            }
            public async Task<Result<DefaultFoodOrder>> Handle(GetByIdDefaultFoodOrderQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<DefaultFoodOrder>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<DefaultFoodOrder>.SuccessAsync(product);
            }
        }
    }
}
