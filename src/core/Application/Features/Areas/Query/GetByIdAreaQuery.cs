using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Areas.Query
{

    public class GetByIdAreaQuery : IRequest<Result<Area>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdAreaQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetAreaByIdQueryHandler : IRequestHandler<GetByIdAreaQuery, Result<Area>>
        {
            private readonly IRepositoryAsync<Area> _repository;
            public GetAreaByIdQueryHandler(IRepositoryAsync<Area> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Area>> Handle(GetByIdAreaQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<Area>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<Area>.SuccessAsync(product);
            }
        }
    }
}
