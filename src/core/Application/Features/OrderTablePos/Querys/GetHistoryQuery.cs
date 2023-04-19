using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTablePos.Querys
{

    public class GetHistoryQuery : IRequest<Result<OrderTable>>
    {
        public Guid IdOrder { get; set; }
        public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, Result<OrderTable>>
        {
            private readonly IRepositoryAsync<OrderTable> _repository;
            private readonly IRepositoryAsync<RoomAndTable> _RoomAndTablerepository;
            private readonly IManagerInvNoRepository _managerInvNoRepository;

            public GetHistoryQueryHandler(IRepositoryAsync<OrderTable> repository, IRepositoryAsync<RoomAndTable> RoomAndTablerepository,
                IManagerInvNoRepository managerInvNoRepository)
            {
                _RoomAndTablerepository = RoomAndTablerepository;
                _managerInvNoRepository = managerInvNoRepository;
                _repository = repository;
            }
            public async Task<Result<OrderTable>> Handle(GetHistoryQuery query, CancellationToken cancellationToken)
            {

                var product = await _repository.GetAllQueryable().Include(x => x.HistoryOrders).SingleOrDefaultAsync(x => x.IdGuid == query.IdOrder);

                return await Result<OrderTable>.SuccessAsync(product);
            }
        }
    }
}
