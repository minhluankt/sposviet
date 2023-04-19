using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Supplierss.Query
{

    public class GetByIdSuppliersQuery : IRequest<Result<Suppliers>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdSuppliersQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetSuppliersByIdQueryHandler : IRequestHandler<GetByIdSuppliersQuery, Result<Suppliers>>
        {
            private readonly IRepositoryAsync<Suppliers> _repository;
            public GetSuppliersByIdQueryHandler(IRepositoryAsync<Suppliers> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Suppliers>> Handle(GetByIdSuppliersQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<Suppliers>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<Suppliers>.SuccessAsync(product);
            }
        }
    }
}
