using Application.DTOs.Logs;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;

namespace Application.Features.Logs.Queries
{

    public class GetAllLogsQuery : IRequest<Result<IPagedList<AuditLogResponse>>>
    {
        public string textSearch { get; set; }
        public string ToDate { get; set; }
        public string FromDate { get; set; }
        public string UserId { get; set; }
        public int ComId { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }

        public GetAllLogsQuery()
        {
        }
    }

    public class GetAllLogsQueryHandler : IRequestHandler<GetAllLogsQuery, Result<IPagedList<AuditLogResponse>>>
    {

        private readonly ILogRepository _repo;
        private readonly IMapper _mapper;

        public GetAllLogsQueryHandler(ILogRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Result<IPagedList<AuditLogResponse>>> Handle(GetAllLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repo.GetAuditLogsPaginated(request.ComId, request.UserId, request.FromDate, request.ToDate, request.textSearch, request.pageIndex, request.pageSize);

            return Result<IPagedList<AuditLogResponse>>.Success(logs);
        }
    }
}
