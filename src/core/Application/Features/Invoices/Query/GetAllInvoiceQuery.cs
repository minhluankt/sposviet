using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Query
{
    public class GetAllInvoiceQuery : DatatableModel, IRequest<Result<PaginatedList<Invoice>>>
    {
        public InvoiceModel invoiceModel { get; set; }
        public GetAllInvoiceQuery(int _comId)
        {
            Comid = _comId;
        }
    }

    public class GetAllInvoicedQueryHandler : IRequestHandler<GetAllInvoiceQuery, Result<PaginatedList<Invoice>>>
    {
        private readonly IInvoicePepository<Invoice> _Invoice;
        private readonly IMapper _mapper;

        public GetAllInvoicedQueryHandler(
            IMapper mapper, IInvoicePepository<Invoice> Invoice)
        {
            _Invoice = Invoice;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<Invoice>>> Handle(GetAllInvoiceQuery request, CancellationToken cancellationToken)
        {
            return await Result<PaginatedList<Invoice>>.SuccessAsync(await _Invoice.GetAllDatatableAsync(request.Comid, request.invoiceModel, request.sortColumn, request.sortColumnDirection, request.pageSize, request.skip, request.TypeProduct));
        }
    }
}
