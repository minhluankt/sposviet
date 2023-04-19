using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.SupplierEInvoices.Query
{

    public class GetAllSupplierEInvoiceQuery : DatatableModel, IRequest<Result<List<SupplierEInvoiceModel>>>
    {
        public bool IsManagerPatternEInvoices { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public GetAllSupplierEInvoiceQuery()
        {
        }
    }

    public class GetAllSupplierEInvoicedQueryHandler : IRequestHandler<GetAllSupplierEInvoiceQuery, Result<List<SupplierEInvoiceModel>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _repository;
        private readonly IMapper _mapper;

        public GetAllSupplierEInvoicedQueryHandler(ISupplierEInvoiceRepository<SupplierEInvoice> repository,
             IOptions<CryptoEngine.Secrets> config,
            IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<List<SupplierEInvoiceModel>>> Handle(GetAllSupplierEInvoiceQuery request, CancellationToken cancellationToken)
        {

            List<SupplierEInvoiceModel> productList = new List<SupplierEInvoiceModel>();
            var datalist = _repository.Entities.Include(x=>x.ManagerPatternEInvoices).Where(x=>x.ComId==request.Comid).OrderByDescending(m => m.Id).AsNoTracking();
            
            if (request.TypeSupplierEInvoice!=ENumSupplierEInvoice.NONE)
            {
                datalist = datalist.Where(x => x.TypeSupplierEInvoice== request.TypeSupplierEInvoice);
            }
            if (request.IsManagerPatternEInvoices)
            {
                datalist = datalist.Include(x => x.ManagerPatternEInvoices);
            }
            if (datalist.Count()==0)
            {
                return await Result<List<SupplierEInvoiceModel>>.FailAsync("Hãy cấu hình hóa đơn điện tử trước khi phát hành hóa đơn");
            }
            productList = datalist.ToList().Select(x => new
            SupplierEInvoiceModel
            {
                Id = x.Id,
                DomainName = x.DomainName,
                TypeSupplierEInvoice = x.TypeSupplierEInvoice,
                UserNameAdmin = x.UserNameAdmin,
                PassWordService = x.PassWordService,
                UserNameService = x.UserNameService,
                PassWordAdmin = x.PassWordAdmin,
                TypeSeri = x.TypeSeri,
                Active = x.Active,
                Selected = x.Selected,
                ManagerPatternEInvoices = x.ManagerPatternEInvoices?.ToList(),
            }).ToList();
            foreach (var item in productList)
            {
                var values = "id=" + item.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);

                var valuestype = "type=" + (int)item.TypeSupplierEInvoice;
                var screcttype = CryptoEngine.Encrypt(valuestype, _config.Value.Key);
                item.CompanyName = LibraryCommon.GetDisplayNameEnum(item.TypeSupplierEInvoice);
                item.screct = secret;
                item.screcttype = screcttype;
            }

            return await Result<List<SupplierEInvoiceModel>>.SuccessAsync(productList);
        }
    }
}
