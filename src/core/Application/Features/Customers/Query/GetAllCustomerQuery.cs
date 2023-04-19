using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Customers.Query
{

    public class GetAllCustomerQuery : DatatableModel, IRequest<Result<List<CustomerModel>>>
    {
        public string Name { get; set; }
        public string TextPhoneOrEmail { get; set; }
        public bool GetInvoice { get; set; }
        public GetAllCustomerQuery()
        {
        }
    }

    public class GetAllCustomerCachedQueryHandler : IRequestHandler<GetAllCustomerQuery, Result<List<CustomerModel>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<Customer> _repository;
        private readonly ICustomerCacheRepository _companyCache;
        private readonly IMapper _mapper;

        public GetAllCustomerCachedQueryHandler(ICustomerCacheRepository companyCacheCache,
             IOptions<CryptoEngine.Secrets> config,
            IMapper mapper, IRepositoryAsync<Customer> repository)
        {
            _config = config;
            _companyCache = companyCacheCache;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<List<CustomerModel>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        {

            int coute = 0;
            List<CustomerModel> productList = new List<CustomerModel>();

            var datalist = _repository.GetAllQueryable().AsNoTracking();

            if (request.Comid > 0)
            {
                datalist = datalist.Where(x => x.Comid == request.Comid);
            }
            if (!string.IsNullOrEmpty(request.TextPhoneOrEmail))
            {
                datalist = datalist.Where(m => m.Email.ToLower().Contains(request.TextPhoneOrEmail.ToLower()) || m.PhoneNumber.ToLower().Contains(request.TextPhoneOrEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()) || m.UserName.ToLower().Contains(request.Name.ToLower()));
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
            datalist = datalist.Skip(request.skip).Take(request.pageSize).Include(m => m.District);
            if (request.GetInvoice)
            {
                datalist = datalist.Include(x => x.Invoices);
            }

            if (coute > 0)
            {
                productList = datalist.Select(x => new
                     CustomerModel
                {
                    Name = x.Name,
                    UserName = x.UserName,
                    LoginProvider = x.LoginProvider,
                    Status = x.Status,
                    LoginLast = x.LoginLast,
                    Address = x.Address,
                    Image = x.Image,
                    Logo = x.Logo,
                    Code = x.Code,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    Id = x.Id,
                    IdCodeGuid = x.IdCodeGuid,
                    Total = (x.Invoices != null ? x.Invoices.Sum(x => x.Amonut) : 0),
                    createdate = x.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                }).ToList();
                foreach (var item in productList)
                {
                    var values = "id=" + item.Id;
                    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    item.UrlParameters = secret;
                }
            }

            return await Result<List<CustomerModel>>.SuccessAsync(productList, coute.ToString());
        }
    }
}
