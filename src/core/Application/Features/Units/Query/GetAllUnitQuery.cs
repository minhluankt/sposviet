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
using Unit = Domain.Entities.Unit;

namespace Application.Features.Units.Query
{
    public class GetAllUnitQuery : DatatableModel, IRequest<Result<List<Unit>>>
    {
        public string Name { get; set; }
        public int IdUnit { get; set; }
        public int ComId { get; set; }
        public bool Paging { get; set; } = true;
        public bool IncludeOrdertable { get; set; } = false;

        public GetAllUnitQuery(int _comId)
        {

            ComId = _comId;
        }
    }

    public class GetAllUnitCachedQueryHandler : IRequestHandler<GetAllUnitQuery, Result<List<Unit>>>
    {

        private readonly IRepositoryAsync<Unit> _repository;
        private readonly IMapper _mapper;

        public GetAllUnitCachedQueryHandler(
            IRepositoryAsync<Unit> repository,


            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<Unit>>> Handle(GetAllUnitQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Unit> lst = _repository.GetAllQueryable().Where(x => x.ComId == request.ComId).AsNoTracking();
            if (!string.IsNullOrEmpty(request.Name))
            {
                lst = lst.Where(x => x.Name.ToLower().Contains(request.Name.ToLower()));
            }
            lst = lst.Include(x=>x.Products);
            if (!request.Paging)
            {
                return Result<List<Unit>>.Success(await lst.OrderByDescending(m => m.Id).ToListAsync(), lst.Count().ToString());
            }
            return Result<List<Unit>>.Success(await lst.OrderByDescending(m => m.Id).Skip(request.skip).Take(request.pageSize).ToListAsync(), lst.Count().ToString());
        }
    }
}
