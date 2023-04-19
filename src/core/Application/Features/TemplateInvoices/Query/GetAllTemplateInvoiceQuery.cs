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


namespace Application.Features.TemplateInvoices.Query
{

    public class GetAllTemplateInvoiceQuery : DatatableModel, IRequest<Result<List<TemplateInvoiceModel>>>
    {
        public string Name { get; set; }
        public string TextPhoneOrEmail { get; set; }
        public bool GetInvoice { get; set; }
        public GetAllTemplateInvoiceQuery()
        {
        }
    }

    public class GetAllTemplateInvoicedQueryHandler : IRequestHandler<GetAllTemplateInvoiceQuery, Result<List<TemplateInvoiceModel>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly ITemplateInvoiceRepository<TemplateInvoice> _repository;
        private readonly IMapper _mapper;

        public GetAllTemplateInvoicedQueryHandler(ITemplateInvoiceRepository<TemplateInvoice> repository,
             IOptions<CryptoEngine.Secrets> config,
            IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<List<TemplateInvoiceModel>>> Handle(GetAllTemplateInvoiceQuery request, CancellationToken cancellationToken)
        {

            int coute = 0;
            List<TemplateInvoiceModel> productList = new List<TemplateInvoiceModel>();

            var datalist = _repository.GetAllAsync(request.Comid).AsNoTracking();

            if (!string.IsNullOrEmpty(request.Name))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDirection)))
            {
                datalist = datalist.OrderBy(request.sortColumn + " " + request.sortColumnDirection);
            }
            else
            {
                datalist = datalist.OrderByDescending(m => m.Id);
            }

            coute = datalist.Count();
            request.recordsTotal = coute;
            datalist = datalist.Skip(request.skip).Take(request.pageSize);

            if (coute > 0)
            {
                productList = datalist.Select(x => new
                     TemplateInvoiceModel
                {
                    Name = x.Name,
                    Active = x.Active,
                    Id = x.Id,
                    Date = x.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                }).ToList();
                foreach (var item in productList)
                {
                    var values = "id=" + item.Id;
                    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    item.screct = secret;
                }
            }

            return await Result<List<TemplateInvoiceModel>>.SuccessAsync(productList, coute.ToString());
        }
    }
}
