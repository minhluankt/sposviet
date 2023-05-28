using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Customers.Query
{

    public class SearchCustomerQuery : IRequest<Result<List<Customer>>>
    {
        public int Comid { get; set; }
        public string Name { get; set; }
        public string sortDirection { get; set; }
        public  AutocompleteTypeCustomer type{ get; set; } = AutocompleteTypeCustomer.NONE;
        public SearchCustomerQuery()
        {
        }
    }

    public class SearchCustomerQueryHandler : IRequestHandler<SearchCustomerQuery, Result<List<Customer>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<Customer> _repository;
        private readonly ICustomerRepository _repositoryCustomer;
        private readonly IMapper _mapper;

        public SearchCustomerQueryHandler(ICustomerRepository companyCacheCache,
             IOptions<CryptoEngine.Secrets> config,
            IMapper mapper, IRepositoryAsync<Customer> repository)
        {
            _config = config;
            _repositoryCustomer = companyCacheCache;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<List<Customer>>> Handle(SearchCustomerQuery request, CancellationToken cancellationToken)
        {
            var datalist = _repository.GetAllQueryable().AsNoTracking().Where(x => x.Comid == request.Comid);
            if (!string.IsNullOrEmpty(request.Name) && request.type ==AutocompleteTypeCustomer.NONE)
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()) || m.Taxcode.ToLower().Contains(request.Name.ToLower()) || m.Code.ToLower().Contains(request.Name.ToLower()));
            }
            else if (request.type == AutocompleteTypeCustomer.TAXCODE)
            {
                datalist = datalist.Where(m => m.Taxcode.ToLower().Contains(request.Name.ToLower()));
            }
            else if (request.type == AutocompleteTypeCustomer.CUSNAME)
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }
            else if (request.type == AutocompleteTypeCustomer.BUYER)
            {
                datalist = datalist.Where(m => m.Buyer.ToLower().Contains(request.Name.ToLower()));
            }
            else if (request.type == AutocompleteTypeCustomer.CUSCODE)
            {
                datalist = datalist.Where(m => m.Code.ToLower().Contains(request.Name.ToLower()));
            }
            return await Result<List<Customer>>.SuccessAsync(datalist.OrderBy(x => x.Name).Take(15).ToList());
        }

    }
}
