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
namespace Application.Features.Logs.QueriesDapper
{

    public class GetAllQuery : IRequest<Result<IPagedList<AuditLogByUser>>>
    {
        public string textSearch { get; set; }
        public string ToDate { get; set; }
        public string FromDate { get; set; }
        public string UserId { get; set; }
        public int pageIndex { get; set; }
        public int ComId { get; set; }
        public int pageSize { get; set; }

        public GetAllQuery()
        {
        }
    }

    public class GetAllLogsQueryHandler : IRequestHandler<GetAllQuery, Result<IPagedList<AuditLogByUser>>>
    {

        private readonly IDapperRepository _dapperdb;
        private readonly ILogRepository _repo;
        private readonly IMapper _mapper;

        public GetAllLogsQueryHandler(ILogRepository repo, IMapper mapper, IDapperRepository dapperdb)
        {
            _repo = repo;
            _mapper = mapper;
            _dapperdb = dapperdb;
        }

        public async Task<Result<IPagedList<AuditLogByUser>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repo.GetAuditLogsDapperPaginated(request.ComId, request.UserId, request.FromDate, request.ToDate, request.textSearch, request.pageIndex, request.pageSize);

            return Result<IPagedList<AuditLogByUser>>.Success(logs);
        }
    }
}
