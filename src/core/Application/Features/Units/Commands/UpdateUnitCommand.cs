using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unit = Domain.Entities.Unit;

namespace Application.Features.Units.Commands
{
    public partial class UpdateUnitCommand : Unit, IRequest<Result<int>>
    {
        public UpdateUnitCommand(int _comId)
        {
            ComId = _comId;
        }
        public IFormFile Img { get; set; }
    }
    public class UpdateUnitHandler : IRequestHandler<UpdateUnitCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Unit> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateUnitHandler(IRepositoryAsync<Unit> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateUnitCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.Code = Common.ConvertToSlug(command.Name);
                brand.FullName = command.FullName;
                brand.Name = command.Name;
                var checkcode = _Repository.Entities.Count(predicate: m => m.Code == brand.Code && m.Id != brand.Id);
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
