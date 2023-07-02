using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BarAndKitchens.Commands
{
    public partial class CreateBarAndKitchenCommand : BarAndKitchen, IRequest<Result<int>>
    {
        public IFormFile Img { get; set; }
    }
    public class CreateBarAndKitchenHandler : IRequestHandler<CreateBarAndKitchenCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<BarAndKitchen> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateBarAndKitchenHandler(IRepositoryAsync<BarAndKitchen> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateBarAndKitchenCommand request, CancellationToken cancellationToken)
        {
            request.Name = request.Name;
            request.Slug = Common.ConvertToSlug(request.Name);
            request.Active = true;
            var fidn = _Repository.Entities.AsNoTracking().Where(m => m.Slug == request.Slug && m.ComId==request.ComId).SingleOrDefault();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
            var product = _mapper.Map<BarAndKitchen>(request);
            await _Repository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
