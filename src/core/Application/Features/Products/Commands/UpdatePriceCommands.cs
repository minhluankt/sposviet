using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands
{
   
    public class UpdatePriceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public decimal Price { get; set; }
        public decimal VATRate { get; set; }
        public decimal PriceNoVAT { get; set; }
        public EnumTypeUpdatePriceProduct TypeUpdatePriceProduct { get; set; } = EnumTypeUpdatePriceProduct.PRICE;
        public class UpdatePriceHandler : IRequestHandler<UpdatePriceCommand, Result<int>>
        {
            private readonly ITableLinkRepository _tablelink;
            private readonly ILogger<UpdatePriceHandler> _log;
            private readonly IRepositoryAsync<Product> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly IFormFileHelperRepository _fileHelper;
            private IUnitOfWork _unitOfWork { get; set; }

            public UpdatePriceHandler(IRepositoryAsync<Product> Repository,
                 IFormFileHelperRepository fileHelper, ITableLinkRepository tablelink,
                ILogger<UpdatePriceHandler> log, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _log = log;
                _Repository = Repository;
                _fileHelper = fileHelper; _tablelink = tablelink;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(UpdatePriceCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.SingleByExpressionAsync(x=>x.Id== command.Id&&x.ComId==command.ComId);
                if (product != null)
                {
                    if (command.TypeUpdatePriceProduct==EnumTypeUpdatePriceProduct.PRICE)
                    {
                        product.Price = command.Price;
                    }
                    else if (command.TypeUpdatePriceProduct == EnumTypeUpdatePriceProduct.VATPRICE)
                    {
                        product.VATRate = command.VATRate;
                        if (command.VATRate > 0 && command.PriceNoVAT <= 0)
                        {
                            return Result<int>.Fail("Đơn giá trước thuế không hợp lệ khi có thuế suất");
                        }
                        if (command.VATRate==(decimal)NOVAT.NOVAT)
                        {
                            product.PriceNoVAT = 0;
                        }
                        else
                        {
                            product.PriceNoVAT = command.PriceNoVAT;
                        }
                    }
                    else if (command.TypeUpdatePriceProduct == EnumTypeUpdatePriceProduct.PRICENOVAT)
                    {
                       
                        if (command.VATRate==(decimal)NOVAT.NOVAT)
                        {
                            return Result<int>.Fail("Vui lòng chọn thuế suất");
                        }
                        if (command.PriceNoVAT == 0 && command.VATRate > 0)
                        {
                            return Result<int>.Fail("Đơn giá trước thuế không hợp lệ khi có thuế suất");
                        }

                        product.PriceNoVAT = command.PriceNoVAT;
                    }
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }

                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
