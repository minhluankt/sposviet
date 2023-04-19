using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.NotificationNewsEmails.Commands
{
    public partial class CreateNotificationNewsEmailCommand : IRequest<Result<int>>
    {
        public string Email { get; set; }
    }
    public class CreateNotificationNewsEmailHandler : IRequestHandler<CreateNotificationNewsEmailCommand, Result<int>>
    {
        private readonly IRepositoryAsync<NotificationNewsEmail> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateNotificationNewsEmailHandler(IRepositoryAsync<NotificationNewsEmail> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateNotificationNewsEmailCommand request, CancellationToken cancellationToken)
        {
            var fidn =  _Repository.Entities.Where(m=>m.Email == request.Email).SingleOrDefault();
            if (fidn!=null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR005);
            }
            var product = new NotificationNewsEmail()
            {
                Email = request.Email,
                CreatedBy = "System",
            };
            await _Repository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
