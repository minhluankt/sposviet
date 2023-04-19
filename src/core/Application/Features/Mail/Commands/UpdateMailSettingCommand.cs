using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
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
using SystemVariable;

namespace Application.Features.Mail.Commands
{

    public partial class UpdateMailSettingCommand : MailSettings, IRequest<Result<int>>
    {
        public bool ChangePass { get; set; }
    }
    public class UpdateMailSettingHandler : IRequestHandler<UpdateMailSettingCommand, Result<int>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<MailSettings> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateMailSettingHandler(IRepositoryAsync<MailSettings> brandRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateMailSettingCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
            else
            {
                if (!string.IsNullOrEmpty(command.Password) && command.ChangePass)
                {
                    brand.Password = EncryptionHelper.Encrypt(command.Password, SystemVariableHelper.publicKey);
                }
                brand.UserName = command.UserName;
                brand.Host = command.Host;
                brand.Port = command.Port;
                brand.From = command.From;
                brand.DisplayName = command.DisplayName;
                await _Repository.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _distributedCache.RemoveAsync(MailSetting.Getkey);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
