using Application.Extensions;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PaymentMethods.Query
{
    public class GetAllPaymentMethodQuery : DatatableModel,IRequest<Result<IQueryable<PaymentMethod>>>
    {
     
        public string Keyword { get; set; }
        public GetAllPaymentMethodQuery()
        {
          
        }
    }
    public class GetAllPaymentMethodsQueryHandler : IRequestHandler<GetAllPaymentMethodQuery, Result<IQueryable<PaymentMethod>>>
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPaymentMethodsQueryHandler(IMapper mapper, IPaymentMethodRepository repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<PaymentMethod>>> Handle(GetAllPaymentMethodQuery request, CancellationToken cancellationToken)
        {
            var productList =  _repository.GetAll(request.Comid);
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                productList = productList.Where(x => x.Name.ToLower().Contains(request.Keyword.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.sortColumn))
            {
                productList = productList.OrderBy(request.sortColumn + " " + request.sortColumnDirection);
            }
            else
            {
                productList = productList.OrderBy(x=>x.Name);
            }

            return await Result<IQueryable<PaymentMethod>>.SuccessAsync(productList);
        }
    }
}
