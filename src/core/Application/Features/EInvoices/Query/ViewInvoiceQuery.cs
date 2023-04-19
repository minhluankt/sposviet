using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.EInvoices.Query
{
  
    public class ViewInvoiceQuery : IRequest<IResult<string>>
    {
        public string IdCarsher { get; set; }
        public string Carsher { get; set; }
        public int[] lstid { get; set; }
        public int IdEInvoice { get; set; }
        public ENumTypePrint typePrint { get; set; }
        public int ComId { get; set; }
        public ENumTypeEventEinvoicePosrtal TypeEventEinvoicePosrtal { get; set; }

        public class ViewInvoiceQueryHandler : IRequestHandler<ViewInvoiceQuery, IResult<string>>
        {
            private readonly IRepositoryAsync<Domain.Entities.EInvoice> _repository;
            private readonly IEInvoiceRepository<Domain.Entities.EInvoice> _EInvoicerepository;
            public ViewInvoiceQueryHandler(IRepositoryAsync<Domain.Entities.EInvoice> repository, IEInvoiceRepository<Domain.Entities.EInvoice> EInvoicerepository)
            {
                _EInvoicerepository = EInvoicerepository;
                _repository = repository;
            }
            public async Task<IResult<string>> Handle(ViewInvoiceQuery query, CancellationToken cancellationToken)
            {
                if (query.TypeEventEinvoicePosrtal ==ENumTypeEventEinvoicePosrtal.Print && query.lstid != null)
                {
                    return await _EInvoicerepository.ConvertForStoreFkeyMutiInvoiceAsync(query.lstid, query.ComId, query.typePrint,query.Carsher,query.IdCarsher);
                }
                else if (query.TypeEventEinvoicePosrtal == ENumTypeEventEinvoicePosrtal.Print|| query.TypeEventEinvoicePosrtal == ENumTypeEventEinvoicePosrtal.ExportPDF)
                {
                    return await _EInvoicerepository.ConvertForStoreFkeyAsync(query.IdEInvoice, query.ComId, query.Carsher, query.IdCarsher);
                }
                else if (query.TypeEventEinvoicePosrtal == ENumTypeEventEinvoicePosrtal.ExportXML)
                {
                    return await _EInvoicerepository.DownloadInvFkeyNoPayAsync(query.IdEInvoice, query.ComId, query.Carsher, query.IdCarsher);
                }
                return await _EInvoicerepository.GetInvViewFkeyAsync(query.IdEInvoice, query.ComId, query.Carsher, query.IdCarsher);
               
            }
        }
    }
}
