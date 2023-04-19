using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.Extensions.Options;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Query
{

    public class GetOrderQueryQuery : IRequest<Result<CustomerModelView>>
    {
        public int IdCustomer { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string codeOrder { get; set; }
        public string sortColumn { get; set; }
        public int recordsTotal { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public int Status { get; set; }
        public string sortColumnDirection { get; set; }

        public class GetCustomerByIdQueryHandler : IRequestHandler<GetOrderQueryQuery, Result<CustomerModelView>>
        {
            private IOptions<CryptoEngine.Secrets> _config;
            private readonly IRepositoryAsync<Customer> _repository;
            private readonly IOrderRepository<Order> _repositoryOrder;

            public GetCustomerByIdQueryHandler(IRepositoryAsync<Customer> repository,
                   IOptions<CryptoEngine.Secrets> config,
                IOrderRepository<Order> repositoryOrder)
            {
                _config = config;
                _repositoryOrder = repositoryOrder;
                _repository = repository;
            }
            public async Task<Result<CustomerModelView>> Handle(GetOrderQueryQuery request, CancellationToken cancellationToken)
            {
                CustomerModelView customerModelView = new CustomerModelView();
                var datalist = _repositoryOrder.GetAllOrder();

                if (request.IdCustomer > 0)
                {
                    datalist = datalist.Where(m => m.IdCustomer == request.IdCustomer);
                }
                if (!string.IsNullOrEmpty(request.codeOrder))
                {
                    datalist = datalist.Where(m => m.OrderCode.ToLower().Contains(request.codeOrder.ToLower()));
                }

                if (request.FromDate != null && request.ToDate != null)
                {
                    datalist = datalist.Where(m => m.CreatedOn >= request.FromDate && m.CreatedOn < request.ToDate.Value.AddDays(1));
                }

                if (request.Status > 0)
                {
                    datalist = datalist.Where(m => m.Status == request.Status);
                }

                customerModelView.TotalRow = datalist.Count();
                if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDirection)))
                {
                    datalist = datalist.OrderBy(request.sortColumn + " " + request.sortColumnDirection);
                }
                else
                {
                    datalist = datalist.OrderByDescending(m => m.Id);
                }
                datalist = datalist.Skip(request.skip).Take(request.pageSize);
                if (datalist.Count() > 0)
                {
                    customerModelView.Orders = datalist.Select(x => new
                         OrderViewModel
                    {
                        Id = x.Id,
                        CusName = x.CusName,
                        CusCode = x.CusCode,
                        PhoneNumber = x.PhoneNumber,
                        Address = x.Address,
                        OrderCode = x.OrderCode,
                        CodeVoucher = x.CodeVoucher,
                        Status = x.Status,
                        Amount = x.Amount,
                        AmountInWord = x.AmountInWord,
                        Quantity = x.Quantity,
                        CreatedOn = x.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                    }).ToList();
                    foreach (var item in customerModelView.Orders)
                    {
                        var values = "id=" + item.Id;
                        var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                        item.secretId = secret;

                        var valuescode = "code=" + item.OrderCode;
                        var secretcode = CryptoEngine.Encrypt(valuescode, _config.Value.Key);
                        item.secretCode = secretcode;

                        item.StatusName = Application.Hepers.GeneralMess.ConvertStatusOrderHtml((EnumStatusOrder)item.Status);

                    }
                }
                else
                {
                    customerModelView.Orders = new List<OrderViewModel>();
                }

                return await Result<CustomerModelView>.SuccessAsync(customerModelView);
            }
        }
    }
}
