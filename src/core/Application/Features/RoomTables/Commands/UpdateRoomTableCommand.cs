using Application.CacheKeys;
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

namespace Application.Features.RoomAndTables.Commands
{
    public partial class UpdateRoomAndTableCommand : RoomAndTable, IRequest<Result<int>>
    {
        public UpdateRoomAndTableCommand(int _comId)
        {
            ComId = _comId;
        }
        public IFormFile Img { get; set; }
    }
    public class UpdateRoomAndTableHandler : IRequestHandler<UpdateRoomAndTableCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<RoomAndTable> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateRoomAndTableHandler(IRepositoryAsync<RoomAndTable> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateRoomAndTableCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.Slug = Common.ConvertToSlug(command.Name);
                brand.STT = command.STT;
                brand.Name = command.Name;
                brand.Note = command.Note;
                brand.Active = command.Active;
                brand.IsUse = command.IsUse;
                brand.IdArea = command.IdArea;
                brand.NumberSeats = command.NumberSeats;
                var checkcode = _Repository.Entities.Count(predicate: m => m.Slug == brand.Slug && m.Id != brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(RoomAndTableCacheKeys.RoomAndTableList(brand.ComId));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
