using Application.CacheKeys;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RoomAndTables.Commands
{
    public partial class CreateRoomAndTableCommand : RoomAndTable, IRequest<Result<int>>
    {
        public CreateRoomAndTableCommand(int _comId)
        {
            ComId = _comId;
        }
    }
    public class CreateRoomAndTableHandler : IRequestHandler<CreateRoomAndTableCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<RoomAndTable> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateRoomAndTableHandler(IRepositoryAsync<RoomAndTable> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateRoomAndTableCommand request, CancellationToken cancellationToken)
        {
            List<RoomAndTable> roomAnds = new List<RoomAndTable>();
            var fidn = _Repository.Entities.Where(m => m.ComId == request.ComId).ToList();
            if (request.IsCreateMuti)
            {
                string error = string.Empty;
                for (int i = request.Fromno; i <= request.Tono; i++)
                {
                    string name = request.NameSelect + " " + i;
                    string Slug = Common.ConvertToSlug(name);
                    if (fidn.Select(x => x.Slug).Contains(Slug))
                    {
                        error += name + ",";
                    }
                    else
                    {
                        roomAnds.Add(new RoomAndTable()
                        {
                            Name = name,
                            Slug = Slug,
                            IdArea = request.IdArea,
                            NumberSeats = request.NumberSeats,
                            Active = request.Active,
                            Note = request.Note,
                            ComId = request.ComId
                        });
                    }
                }
                if (!string.IsNullOrEmpty(error))
                {
                    return await Result<int>.FailAsync("Tên dãy phòng/bàn đã tồn tại!<br/>" + error);
                }
            }
            else
            {
                request.Slug = Common.ConvertToSlug(request.Name);
                if (fidn.Select(x => x.Slug).Contains(request.Slug))
                {
                    return await Result<int>.FailAsync("Tên phòng đã tồn tại!");
                }
                var product = _mapper.Map<RoomAndTable>(request);
                roomAnds.Add(product);
            }

            await _Repository.AddRangeAsync(roomAnds);
            await _distributedCache.RemoveAsync(RoomAndTableCacheKeys.RoomAndTableList(request.ComId));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success();
        }
    }
}
