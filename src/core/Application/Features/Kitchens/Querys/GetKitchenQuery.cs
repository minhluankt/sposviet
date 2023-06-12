using Application.Constants;
using Application.Enums;
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

namespace Application.Features.Kitchens.Querys
{
  
    public class GetKitchenQuery : IRequest<Result<Kitchen>>
    {
        public int Comid { get; set; }
        public int IdKitken { get; set; }
        public class GetOrderChitkenQueryHandler : IRequestHandler<GetKitchenQuery, Result<Kitchen>>
        {
            private readonly INotifyChitkenRepository _repository;
            public GetOrderChitkenQueryHandler(INotifyChitkenRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<Kitchen>> Handle(GetKitchenQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _repository.GetByIdAsync(query.Comid, query.IdKitken);
                    if (product == null)
                    {
                        return await Result<Kitchen>.FailAsync(HeperConstantss.ERR012);
                    }
                    return await Result<Kitchen>.SuccessAsync(product);
                }
                catch (Exception e)
                {
                    return await Result<Kitchen>.FailAsync(e.Message);
                }
            }
        }
    }
}
