using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Banners.Query
{

    public class GetByIdBannerQuery : IRequest<Result<Banner>>
    {
        public int Id { get; set; }

        public class GetBannerByIdQueryHandler : IRequestHandler<GetByIdBannerQuery, Result<Banner>>
        {
            private readonly IRepositoryAsync<Banner> _repository;
            public GetBannerByIdQueryHandler(IRepositoryAsync<Banner> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Banner>> Handle(GetByIdBannerQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<Banner>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<Banner>.SuccessAsync(product);
            }
        }
    }
}
