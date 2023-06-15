using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTablePos.Commands
{
  
    public class UpdateServiceFoodByPaymentCommand : IRequest<Result<OrderTable>>
    {
        public int ComId { get; set; }
        public Guid IdOrder { get; set; }
        public class UpdateServiceFoodByPaymentHandler : IRequestHandler<UpdateServiceFoodByPaymentCommand, Result<OrderTable>>
        {

            private readonly IOrderTableRepository _Repository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public UpdateServiceFoodByPaymentHandler(IRepositoryAsync<Product> ProductRepository,
                IOrderTableRepository brandRepository,

                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {

                _ProductRepository = ProductRepository;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<OrderTable>> Handle(UpdateServiceFoodByPaymentCommand command, CancellationToken cancellationToken)
            {
                return await _Repository.UpdateFoodServiceInPaymentAsync(command.ComId, command.IdOrder);
            }
        }
    }
}
