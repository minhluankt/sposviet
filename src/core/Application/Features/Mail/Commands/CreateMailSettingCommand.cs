using Application.CacheKeys;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;

namespace Application.Features.Mail.Commands
{

    public partial class CreateMailSettingCommand : MailSettings, IRequest<Result<int>>
    {
      
    }
    public class CreateMailSettingHandler : IRequestHandler<CreateMailSettingCommand, Result<int>>
    {
        private readonly IRepositoryAsync<MailSettings> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateMailSettingHandler(IRepositoryAsync<MailSettings> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateMailSettingCommand request, CancellationToken cancellationToken)
        {
            request.Password = EncryptionHelper.Encrypt(request.Password, SystemVariableHelper.publicKey);
            var product = _mapper.Map<MailSettings>(request);
            await _Repository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
