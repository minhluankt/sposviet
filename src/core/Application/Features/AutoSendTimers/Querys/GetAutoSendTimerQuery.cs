using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.AutoSendTimers.Query
{

    public class GetAllAutoSendTimerQuery : DatatableModel, IRequest<Result<PaginatedList<AutoSendTimer>>>
    {
        public string keyword { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public GetAllAutoSendTimerQuery()
        {
        }
    }

    public class GetAllAutoSendTimerdQueryHandler : IRequestHandler<GetAllAutoSendTimerQuery, Result<PaginatedList<AutoSendTimer>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IAutoSendTimerRepository<AutoSendTimer> _repository;
        private readonly IMapper _mapper;

        public GetAllAutoSendTimerdQueryHandler(IAutoSendTimerRepository<AutoSendTimer> repository,
             IOptions<CryptoEngine.Secrets> config,
            IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<PaginatedList<AutoSendTimer>>> Handle(GetAllAutoSendTimerQuery request, CancellationToken cancellationToken)
        {
            return await Result<PaginatedList<AutoSendTimer>>.SuccessAsync(await _repository.GetPageList(request.Comid, request.sortColumn, request.sortColumnDirection, request.currentPage, request.pageSize, request.TypeSupplierEInvoice));
        }
    }
}
