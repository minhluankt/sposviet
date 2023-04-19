using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RoomAndTables.Query
{
    public class GetAllRoomAndTableQuery : DatatableModel, IRequest<Result<IQueryable<RoomAndTable>>>
    {
        public string Name { get; set; }
        public int ComId { get; set; }
        public int IdArea { get; set; }
        public bool Cache { get; set; }

        public GetAllRoomAndTableQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetAllRoomAndTableCachedQueryHandler : IRequestHandler<GetAllRoomAndTableQuery, Result<IQueryable<RoomAndTable>>>
    {

        private readonly IRepositoryCacheAsync<RoomAndTable> _RoomAndTableCache;
        private readonly IRepositoryAsync<RoomAndTable> _repository;
        private readonly IMapper _mapper;

        public GetAllRoomAndTableCachedQueryHandler(IRepositoryCacheAsync<RoomAndTable> RoomAndTableCache,
            IRepositoryAsync<RoomAndTable> repository,


            IMapper mapper)
        {
            _repository = repository;
            _RoomAndTableCache = RoomAndTableCache;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<RoomAndTable>>> Handle(GetAllRoomAndTableQuery request, CancellationToken cancellationToken)
        {
            IQueryable<RoomAndTable> lst;

            if (request.Cache)
            {
                var productList = await _RoomAndTableCache.GetCachedListAsync(RoomAndTableCacheKeys.RoomAndTableList(request.ComId));
                lst = productList.AsQueryable();
            }
            else
            {
                lst = _repository.GetAllQueryable().Include(x => x.Area).Where(x => x.ComId == request.ComId).AsNoTracking();
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                lst = lst.Where(x => x.Name.ToLower().Contains(request.Name));
            }
            if (request.IdArea > 0)
            {
                lst = lst.Where(x => x.IdArea == request.IdArea);
            }

            return Result<IQueryable<RoomAndTable>>.Success(lst.OrderByDescending(m => m.Id), lst.Count().ToString());
        }
    }
}
