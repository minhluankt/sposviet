using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BarAndKitchens.Commands
{
     
    public partial class UpdateBarAndKitchenCommand : BarAndKitchen, IRequest<Result<int>>
    {
        public IFormFile Img { get; set; }
    }
    public class UpdateBarAndKitchenHandler : IRequestHandler<UpdateBarAndKitchenCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<BarAndKitchen> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateBarAndKitchenHandler(IRepositoryAsync<BarAndKitchen> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateBarAndKitchenCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                string slug = Common.ConvertToSlug(command.Name);
                var checkcode = await _Repository.Entities.AsNoTracking().CountAsync(predicate: m => m.Slug == slug && m.Id != brand.Id && m.ComId == brand.ComId);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                brand.Active = command.Active;
                brand.Note = command.Note;
                brand.Name = command.Name;
                brand.Slug = slug;
                await _Repository.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
               
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
