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

namespace Application.Features.Supplierss.Query
{
    public class GetAllSuppliersQuery : DatatableModel, IRequest<Result<IQueryable<Suppliers>>>
    {
        public string Name { get; set; }
        public int ComId { get; set; }
        public GetAllSuppliersQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, Result<IQueryable<Suppliers>>>
    {

        private readonly IRepositoryCacheAsync<Suppliers> _SuppliersCache;
        private readonly IRepositoryAsync<Suppliers> _repository;
        private readonly IMapper _mapper;

        public GetAllSuppliersQueryHandler(IRepositoryCacheAsync<Suppliers> SuppliersCache,
            IRepositoryAsync<Suppliers> repository,


            IMapper mapper)
        {
            _repository = repository;
            _SuppliersCache = SuppliersCache;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<Suppliers>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Suppliers> lst= _repository.GetAllQueryable().Where(x => x.ComId == request.ComId).AsNoTracking();
      
            if (!string.IsNullOrEmpty(request.Name))
            {
                lst = lst.Where(x => x.Name.ToLower().Contains(request.Name)||x.CodeSupplier.ToLower().Contains(request.Name)|| x.Phonenumber.ToLower().Contains(request.Name)|| x.Email.ToLower().Contains(request.Name));
            }
            return await Result<IQueryable<Suppliers>>.SuccessAsync(lst.OrderByDescending(m => m.Id), lst.Count().ToString());
        }
    }
}
