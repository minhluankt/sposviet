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
    public class GetKitchenListQuery : IRequest<Result<List<Kitchen>>>
    {
        public bool IsCancel { get; set; }
        public int Comid { get; set; }
        public int[] lstId { get; set; }
        public int?[] lstIdItemOrder { get; set; }

        public class GetKitchenListQueryHandler : IRequestHandler<GetKitchenListQuery, Result<List<Kitchen>>>
        {
            private readonly INotifyChitkenRepository _repository;

            public GetKitchenListQueryHandler(INotifyChitkenRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<List<Kitchen>>> Handle(GetKitchenListQuery query, CancellationToken cancellationToken)
            {
                if (query.lstIdItemOrder!=null && query.lstIdItemOrder.Count()>0 && query.IsCancel)
                {
                    var getdata = await _repository.GetByListIdItemOrder(query.Comid, query.lstIdItemOrder.Select(x => x.Value).ToArray());
                    getdata = getdata.Where(x => x.Quantity>0&&(x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.Processing)).ToList();
                    return await Result<List<Kitchen>>.SuccessAsync(getdata);
                }
                var product = await _repository.GetByListId(query.Comid, query.lstId);
                return await Result<List<Kitchen>>.SuccessAsync(product);
            }
        }
    }
}
