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

namespace Application.Features.CategoryCevenues.Query
{
    public class GetAllCategoryCevenueQuery : DatatableModel, IRequest<Result<IQueryable<CategoryCevenue>>>
    {
        public EnumTypeRevenueExpenditure Type { get; set; }
        public string Name { get; set; }
        public int ComId { get; set; }
       

        public GetAllCategoryCevenueQuery(int _comId, EnumTypeRevenueExpenditure type)
        {
            Type = type;
            ComId = _comId;
        }
    }

    public class GetAllCategoryCevenueCachedQueryHandler : IRequestHandler<GetAllCategoryCevenueQuery, Result<IQueryable<CategoryCevenue>>>
    {

        private readonly IRepositoryAsync<CategoryCevenue> _repository;
        private readonly IMapper _mapper;

        public GetAllCategoryCevenueCachedQueryHandler(
            IRepositoryAsync<CategoryCevenue> repository,


            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<CategoryCevenue>>> Handle(GetAllCategoryCevenueQuery request, CancellationToken cancellationToken)
        {
            IQueryable<CategoryCevenue> lst = _repository.GetAllQueryable().Where(x => x.ComId == request.ComId&&x.Type == request.Type).AsNoTracking();
            if (!string.IsNullOrEmpty(request.Name))
            {
                lst = lst.Where(x => x.Name.ToLower().Contains(request.Name.ToLower()));
            }
            return await Result<IQueryable<CategoryCevenue>>.SuccessAsync(lst);
        }
    }
}
