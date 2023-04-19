using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Dapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PaymentMethods.Commands
{
    public class DeletePaymentMethodCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeletePaymentMethodHandler : IRequestHandler<DeletePaymentMethodCommand, Result<int>>
        {
            private readonly IRepositoryAsync<PaymentMethod> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly IDapperRepository _dapperdb;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeletePaymentMethodHandler(IRepositoryAsync<PaymentMethod> brandRepository, IDapperRepository dapperdb, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperdb = dapperdb;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeletePaymentMethodCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _Repository.GetByIdAsync(command.Id);
                    await _Repository.DeleteAsync(product);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                catch (Exception e)
                {
                    return await Result<int>.FailAsync(e.Message);
                }
               
            }
        }
    }
}
