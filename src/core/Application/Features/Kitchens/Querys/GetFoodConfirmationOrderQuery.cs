using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.KitchenPos.Querys
{
   
    public class GetFoodConfirmationOrderQuery : IRequest<Result<List<Kitchen>>>
    {
        public int Comid { get; set; }
        public Guid idOrder { get; set; }
        public EnumStatusKitchenOrder Status { get; set; } = EnumStatusKitchenOrder.MOI;
        public class GetOrderChitkenQueryHandler : IRequestHandler<GetFoodConfirmationOrderQuery, Result<List<Kitchen>>>
        {
            private readonly INotifyChitkenRepository _repository;

            public GetOrderChitkenQueryHandler(INotifyChitkenRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<List<Kitchen>>> Handle(GetFoodConfirmationOrderQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _repository.GetAllFoodNewByOrder(query.Comid,query.idOrder);
                    return await Result<List<Kitchen>>.SuccessAsync(product);
                }
                catch (Exception e)
                {
                    return await Result<List<Kitchen>>.FailAsync(e.Message);
                }
            }
        }
    }
}
