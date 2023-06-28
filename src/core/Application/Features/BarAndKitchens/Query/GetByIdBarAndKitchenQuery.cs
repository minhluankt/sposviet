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

namespace Application.Features.BarAndKitchens.Query
{

    public class GetByIdBarAndKitchenQuery : IRequest<Result<BarAndKitchen>>
    {
        public int Id { get; set; }
        public int Comid { get; set; }

        public class GetBarAndKitchenByIdQueryHandler : IRequestHandler<GetByIdBarAndKitchenQuery, Result<BarAndKitchen>>
        {
            private readonly IRepositoryAsync<BarAndKitchen> _repository;
            public GetBarAndKitchenByIdQueryHandler(IRepositoryAsync<BarAndKitchen> repository)
            {
                _repository = repository;
            }
            public async Task<Result<BarAndKitchen>> Handle(GetByIdBarAndKitchenQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.Entities.AsNoTracking().SingleOrDefaultAsync(x=>x.ComId==query.Comid && x.Id==query.Id);
                if (product == null)
                {
                    return await Result<BarAndKitchen>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<BarAndKitchen>.SuccessAsync(product);
            }
        }
    }
}
