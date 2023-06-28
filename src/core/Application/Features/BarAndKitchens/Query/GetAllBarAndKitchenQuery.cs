
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;

using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BarAndKitchens.Query
{
    public class GetAllBarAndKitchenQuery : IRequest<Result<List<BarAndKitchen>>>
    {
        public int ComId { get; set; }
        public GetAllBarAndKitchenQuery()
        {
        }
    }

    public class GetAllBarAndKitchendQueryHandler : IRequestHandler<GetAllBarAndKitchenQuery, Result<List<BarAndKitchen>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IBarAndKitchenRepository _BarAndKitchen;
        private readonly IMapper _mapper;

        public GetAllBarAndKitchendQueryHandler(IBarAndKitchenRepository BarAndKitchen, IOptions<CryptoEngine.Secrets> _config, IMapper mapper)
        {
            _BarAndKitchen = BarAndKitchen;
            this._config = _config;
            _mapper = mapper;
        }

        public async Task<Result<List<BarAndKitchen>>> Handle(GetAllBarAndKitchenQuery request, CancellationToken cancellationToken)
        {
            var productList = await _BarAndKitchen.GetList(request.ComId);
            productList.ForEach( x =>
            {
                var values = "id=" + x.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                x.secret = secret;
            });
            return Result<List<BarAndKitchen>>.Success(productList);
        }
    }
}
