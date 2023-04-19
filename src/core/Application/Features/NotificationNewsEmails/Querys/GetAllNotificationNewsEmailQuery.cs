using Application.Extensions;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.NotificationNewsEmails.Query
{
    public class GetAllNotificationNewsEmailQuery : IRequest<PaginatedResult<NotificationNewsEmail>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetAllNotificationNewsEmailQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
    public class GetAllNotificationNewsEmailsQueryHandler : IRequestHandler<GetAllNotificationNewsEmailQuery, PaginatedResult<NotificationNewsEmail>>
    {
        private readonly IRepositoryAsync<NotificationNewsEmail> _repository;
        // private readonly IRepository _repositordy;
        private readonly IMapper _mapper;

        public GetAllNotificationNewsEmailsQueryHandler(IMapper mapper, IRepositoryAsync<NotificationNewsEmail> repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<NotificationNewsEmail>> Handle(GetAllNotificationNewsEmailQuery request, CancellationToken cancellationToken)
        {
            var productList = _repository.GetAllQueryable().AsNoTracking();
            var paginatedList = await productList.ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
