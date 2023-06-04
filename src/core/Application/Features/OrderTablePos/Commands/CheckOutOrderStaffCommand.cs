using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;

namespace Application.Features.OrderTablePos.Commands
{
    public class CheckOutOrderStaffCommand : IRequest<Result<string>>
    {
        public EnumTypeProduct TypeUpdate { get; set; }
        public Guid IdOrder { get; set; }
        public decimal discountPayment { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public int Idpayment { get; set; }
        public int ComId { get; set; }
        public string Cashername { get; set; }
        public string IdCasher { get; set; }
        public class CheckOutOrderStaffHandler : IRequestHandler<CheckOutOrderStaffCommand, Result<string>>
        {
            private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
            private readonly IOrderTableRepository _Repository;
            private readonly ICompanyAdminInfoRepository _companyProductRepository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public CheckOutOrderStaffHandler(IRepositoryAsync<Product> ProductRepository, ICompanyAdminInfoRepository companyProductRepository,
                IOrderTableRepository brandRepository, ITemplateInvoiceRepository<TemplateInvoice> templateInvoicerepository,

                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {

                _companyProductRepository = companyProductRepository;
                _templateInvoicerepository = templateInvoicerepository;
                _ProductRepository = ProductRepository;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<string>> Handle(CheckOutOrderStaffCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.CheckOutOrderStaffAsync(
                    command.ComId,
                    command.Idpayment,
                    command.IdOrder,
                    command.discountPayment,
                    command.Amount,
                    command.Total,
                    command.Cashername,
                    command.IdCasher,
                    command.TypeUpdate
                    );
                if (product.Succeeded)
                {
                    return await Result<string>.SuccessAsync(product.Message);
                }
                return Result<string>.Fail(product.Message);
            }
        }
    }
}
