using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using PagedList.Core;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;
using X.PagedList;

namespace Application.Features.EInvoices.Query
{
    public class GetAllEInvoiceQuery : DatatableModel, IRequest<Result<PaginatedList<Domain.Entities.EInvoice>>>
    {
        public InvoiceModel EInvoiceModel { get; set; }
        public GetAllEInvoiceQuery(int _comId)
        {
            Comid = _comId;
        }
    }

    public class GetAllEInvoicedQueryHandler : IRequestHandler<GetAllEInvoiceQuery, Result<PaginatedList<Domain.Entities.EInvoice>>>
    {
        private readonly IEInvoiceRepository<Domain.Entities.EInvoice> _EInvoice;
        private readonly IMapper _mapper;

        public GetAllEInvoicedQueryHandler(
            IMapper mapper, IEInvoiceRepository<Domain.Entities.EInvoice> EInvoice)
        {
            _EInvoice = EInvoice;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<Domain.Entities.EInvoice>>> Handle(GetAllEInvoiceQuery request, CancellationToken cancellationToken)
        {
            var get = await _EInvoice.GetAllDatatableAsync(request.Comid, request.EInvoiceModel, request.sortColumn, request.sortColumnDirection, request.pageSize, request.skip, request.TypeProduct);
            return await Result<PaginatedList<Domain.Entities.EInvoice>>.SuccessAsync(get);
        }
    }
}
