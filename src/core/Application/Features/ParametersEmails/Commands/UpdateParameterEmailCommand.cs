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


namespace Application.Features.ParametersEmails.Commands
{

    public partial class UpdateParametersEmailCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
        public string Value { get; set; }
    }
    public class UpdateParametersEmailHandler : IRequestHandler<UpdateParametersEmailCommand, Result<int>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<ParametersEmail> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateParametersEmailHandler(IRepositoryAsync<ParametersEmail> brandRepository, 

            IUnitOfWork unitOfWork,IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateParametersEmailCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
            else
            {
                brand.Key = command.Key;
                brand.Title = command.Title;
                brand.Content = command.Content;
                brand.Value = command.Value;
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(ParametersEmailCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
