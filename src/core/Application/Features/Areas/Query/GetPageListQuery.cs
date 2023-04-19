using Application.Constants;
using Application.Hepers;
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

namespace Application.Features.Areas.Query
{
 
    public class GetPageListQuery : EntitySearchModel, IRequest<Result<PaginatedList<AreasModel>>>
    {
        public GetPageListQuery(int _comId)
        {
            Comid = _comId;
        }

        public class GetAreaByIdQueryHandler : IRequestHandler<GetPageListQuery, Result<PaginatedList<AreasModel>>>
        {
            private readonly IAreaRepository _repository;
            public GetAreaByIdQueryHandler(IAreaRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<PaginatedList<AreasModel>>> Handle(GetPageListQuery query, CancellationToken cancellationToken)
            {
               
                var product = await _repository.GetAllAsync(query);
                if (product == null)
                {
                    return await Result<PaginatedList<AreasModel>>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<PaginatedList<AreasModel>>.SuccessAsync(product);
            }
        }
    }
}
