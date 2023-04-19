using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RoomAndTables.Query
{

    public class GetByIdRoomAndTableQuery : IRequest<Result<RoomAndTable>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdRoomAndTableQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetRoomAndTableByIdQueryHandler : IRequestHandler<GetByIdRoomAndTableQuery, Result<RoomAndTable>>
        {
            private readonly IRepositoryAsync<RoomAndTable> _repository;
            public GetRoomAndTableByIdQueryHandler(IRepositoryAsync<RoomAndTable> repository)
            {
                _repository = repository;
            }
            public async Task<Result<RoomAndTable>> Handle(GetByIdRoomAndTableQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<RoomAndTable>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<RoomAndTable>.SuccessAsync(product);
            }
        }
    }
}
