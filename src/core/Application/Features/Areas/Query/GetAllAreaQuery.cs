using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Areas.Query
{
    public class GetAllAreaQuery : DatatableModel, IRequest<Result<List<Area>>>
    {
        public string Name { get; set; }
        public int IdArea { get; set; }
        public int ComId { get; set; }
        public bool Paging { get; set; } = true;
        public bool IncludeOrdertable { get; set; } = false;
        public EnumStatusArea Status { get; set; } = EnumStatusArea.NONE;

        public GetAllAreaQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetAllAreaCachedQueryHandler : IRequestHandler<GetAllAreaQuery, Result<List<Area>>>
    {

        private readonly IRepositoryAsync<Area> _repository;
        private readonly IMapper _mapper;

        public GetAllAreaCachedQueryHandler(
            IRepositoryAsync<Area> repository,


            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<Area>>> Handle(GetAllAreaQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Area> lst = _repository.GetAllQueryable().Where(x => x.ComId == request.ComId).AsNoTracking();
            if (!string.IsNullOrEmpty(request.Name))
            {
                lst = lst.Where(x => x.Name.ToLower().Contains(request.Name.ToLower()));
            }
            if (request.IdArea > 0)
            {
                lst = lst.Where(x => x.Id == request.IdArea);
            }
            if (request.Status != EnumStatusArea.NONE)
            {
                lst = lst.Where(x => x.Status == request.Status);
            }
            if (request.IncludeOrdertable)
            {
                lst = lst.Include(x => x.RoomAndTables).ThenInclude(x=>x.OrderTables);
            }
            else
            {
                lst = lst.Include(x => x.RoomAndTables);
            }
            if (!request.Paging)
            {
                return Result<List<Area>>.Success(await lst.OrderByDescending(m => m.Id).ToListAsync(), lst.Count().ToString());
            }
            return Result<List<Area>>.Success(await lst.OrderByDescending(m => m.Id).Skip(request.skip).Take(request.pageSize).ToListAsync(), lst.Count().ToString());
        }
    }
}
