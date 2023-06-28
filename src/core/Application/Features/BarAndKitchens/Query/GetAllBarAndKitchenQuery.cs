using Application.Keys;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
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
    public class GetAllBarAndKitchenQuery : IRequest<Result<List<Banner>>>
    {
        public GetAllBarAndKitchenQuery()
        {
        }
    }

    public class GetAllBarAndKitchendQueryHandler : IRequestHandler<GetAllBarAndKitchenQuery, Result<List<Banner>>>
    {

        private readonly IRepositoryAsync<BarAndKitchen> _BarAndKitchen;
        private readonly IMapper _mapper;

        public GetAllBarAndKitchendQueryHandler(IRepositoryAsync<BarAndKitchen> BarAndKitchen, IMapper mapper)
        {
            _BarAndKitchen = BarAndKitchen;
            _mapper = mapper;
        }

        public async Task<Result<List<Banner>>> Handle(GetAllBarAndKitchenQuery request, CancellationToken cancellationToken)
        {
            var productList = await _BarAndKitchen.en();
            return Result<List<Banner>>.Success(productList);
        }
    }
}
