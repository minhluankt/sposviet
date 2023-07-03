using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.VietQRs.Query
{

    public class GetByIdVietQRQuery : IRequest<Result<VietQR>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdVietQRQuery(int _comId,int id)
        {
            ComId = _comId;
            this.Id = id;
        }

        public class GetVietQRByIdQueryHandler : IRequestHandler<GetByIdVietQRQuery, Result<VietQR>>
        {
            private readonly IVietQRRepository<VietQR> _repository;
            public GetVietQRByIdQueryHandler(IVietQRRepository<VietQR> repository)
            {
                _repository = repository;
            }
            public async Task<Result<VietQR>> Handle(GetByIdVietQRQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.ComId,query.Id);
                if (product == null)
                {
                    return await Result<VietQR>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<VietQR>.SuccessAsync(product);
            }
        }
    }
}
