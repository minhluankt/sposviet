using Application.Enums;
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


namespace Application.Features.ManagerPatternEInvoices.Query
{

    public class GetAllManagerPatternEInvoiceQuery : DatatableModel, IRequest<Result<IQueryable<ManagerPatternEInvoice>>>
    {
        public string keyword { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public GetAllManagerPatternEInvoiceQuery()
        {
        }
    }

    public class GetAllManagerPatternEInvoicedQueryHandler : IRequestHandler<GetAllManagerPatternEInvoiceQuery, Result<IQueryable<ManagerPatternEInvoice>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> _repository;
        private readonly IMapper _mapper;

        public GetAllManagerPatternEInvoicedQueryHandler(IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> repository,
             IOptions<CryptoEngine.Secrets> config,
            IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<IQueryable<ManagerPatternEInvoice>>> Handle(GetAllManagerPatternEInvoiceQuery request, CancellationToken cancellationToken)
        {
            var datalist = _repository.Entities.Where(x=>x.ComId==request.Comid && x.TypeSupplierEInvoice == request.TypeSupplierEInvoice).OrderByDescending(m => m.Id).AsNoTracking();
            if (!string.IsNullOrEmpty(request.keyword))
            {
                datalist = datalist.Where(x => x.Pattern.ToUpper().Contains(request.keyword.ToUpper())|| x.Serial.ToUpper().Contains(request.keyword.ToUpper()));
            }
            return await Result<IQueryable<ManagerPatternEInvoice>>.SuccessAsync(datalist);
        }
    }
}
