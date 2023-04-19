using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategoryCevenues.Query
{

    public class GetByIdCategoryCevenueQuery : IRequest<Result<CategoryCevenue>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdCategoryCevenueQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetCategoryCevenueByIdQueryHandler : IRequestHandler<GetByIdCategoryCevenueQuery, Result<CategoryCevenue>>
        {
            private readonly IRepositoryAsync<CategoryCevenue> _repository;
            public GetCategoryCevenueByIdQueryHandler(IRepositoryAsync<CategoryCevenue> repository)
            {
                _repository = repository;
            }
            public async Task<Result<CategoryCevenue>> Handle(GetByIdCategoryCevenueQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<CategoryCevenue>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<CategoryCevenue>.SuccessAsync(product);
            }
        }
    }
}
