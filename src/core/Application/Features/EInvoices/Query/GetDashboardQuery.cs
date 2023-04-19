using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.EInvoices.Query
{
 
    public class GetDashboardQuery : InvoiceModel, IRequest<Result<DashboardEInvoiceModel>>
    {
        public int Comid { get; set; }
        public GetDashboardQuery(int _comId)
        {
            Comid = _comId;
        }
    }

    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardEInvoiceModel>>
    {
        private readonly IEInvoiceRepository<Domain.Entities.EInvoice> _EInvoice;
        private readonly IMapper _mapper;

        public GetDashboardQueryHandler(
            IMapper mapper, IEInvoiceRepository<Domain.Entities.EInvoice> EInvoice)
        {
            _EInvoice = EInvoice;
            _mapper = mapper;
        }

        public async Task<Result<DashboardEInvoiceModel>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            if (request.Date==null)
            {
                return await Result<DashboardEInvoiceModel>.FailAsync(HeperConstantss.ERR012);
            }
            var get = await _EInvoice.GetDashboardAsync(request.Comid, request.Date.Value);
            return await Result<DashboardEInvoiceModel>.SuccessAsync(get);
        }
    }
}
