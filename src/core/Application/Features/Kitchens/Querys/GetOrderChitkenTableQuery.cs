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

namespace Application.Features.Kitchens.Querys
{
    public class GetOrderChitkenTableQuery : IRequest<Result<KitChenTableModel>>
    {
        public int Comid { get; set; }
        public bool OrderByDateReady { get; set; }
        public EnumStatusKitchenOrder Status { get; set; } = EnumStatusKitchenOrder.MOI;
        public class GetOrderChitkenQueryHandler : IRequestHandler<GetOrderChitkenTableQuery, Result<KitChenTableModel>>
        {
            private readonly INotifyChitkenRepository _repository;

            public GetOrderChitkenQueryHandler(INotifyChitkenRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<KitChenTableModel>> Handle(GetOrderChitkenTableQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _repository.GetAllNotifyOrderByTable(query.Comid);
                    return await Result<KitChenTableModel>.SuccessAsync(product);
                }
                catch (Exception e)
                {
                    return await Result<KitChenTableModel>.FailAsync(e.Message);
                }
            }
        }
    }
}
