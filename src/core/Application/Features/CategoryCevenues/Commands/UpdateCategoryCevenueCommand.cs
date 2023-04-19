using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategoryCevenues.Commands
{
    public partial class UpdateCategoryCevenueCommand : CategoryCevenue, IRequest<Result<int>>
    {
        public UpdateCategoryCevenueCommand(int _comId)
        {
            ComId = _comId;
        }
        public IFormFile Img { get; set; }
    }
    public class UpdateCategoryCevenueHandler : IRequestHandler<UpdateCategoryCevenueCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<CategoryCevenue> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateCategoryCevenueHandler(IRepositoryAsync<CategoryCevenue> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateCategoryCevenueCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.Name = command.Name;
                brand.Slug = Common.ConvertToSlug($"{command.Name}-{brand.Type}");
                brand.Content = command.Content;
                var checkcode = _Repository.Entities.Count(predicate: m => m.Slug == brand.Slug && m.Id != brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
