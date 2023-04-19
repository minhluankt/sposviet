using Application.Extensions;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Permissions.Query
{
    public class GetAllPermissionQuery : IRequest<PaginatedResult<Permission>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetAllPermissionQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionQuery, PaginatedResult<Permission>>
    {
        private readonly IRepositoryAsync<Permission> _repository;
        // private readonly IRepository _repositordy;
        private readonly IMapper _mapper;

        public GetAllPermissionsQueryHandler(IMapper mapper, IRepositoryAsync<Permission> repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<Permission>> Handle(GetAllPermissionQuery request, CancellationToken cancellationToken)
        {
            var productList = _repository.GetAllQueryable().AsNoTracking();
            var paginatedList = await productList.ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
    //public class GetAllPermissionsCachedResponse
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Barcode { get; set; }
    //    public byte[] Image { get; set; }
    //    public string Description { get; set; }
    //    public decimal Rate { get; set; }
    //    public int BrandId { get; set; }
    //}
}
