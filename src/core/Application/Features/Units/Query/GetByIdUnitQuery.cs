using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unit = Domain.Entities.Unit;

namespace Application.Features.Units.Query
{

    public class GetByIdUnitQuery : IRequest<Result<Unit>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdUnitQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetUnitByIdQueryHandler : IRequestHandler<GetByIdUnitQuery, Result<Unit>>
        {
            private readonly IRepositoryAsync<Unit> _repository;
            public GetUnitByIdQueryHandler(IRepositoryAsync<Unit> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Unit>> Handle(GetByIdUnitQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<Unit>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<Unit>.SuccessAsync(product);
            }
        }
    }
}
