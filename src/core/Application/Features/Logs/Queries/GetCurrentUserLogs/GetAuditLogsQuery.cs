using Application.DTOs;
using Application.DTOs.Logs;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ActivityLog.Queries.GetUserLogs
{
    public class GetAuditLogsQuery : IRequest<Result<List<AuditLogResponse>>>
    {
        public int ComId { get; set; }
        public string userId { get; set; }

        public GetAuditLogsQuery()
        {
        }
    }

    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<List<AuditLogResponse>>>
    {
        private readonly IRepositoryAsync<AuditLogResponse> _Repository;
        private readonly ILogRepository _repo;
        private readonly IMapper _mapper;

        public GetAuditLogsQueryHandler(ILogRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Result<List<AuditLogResponse>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repo.GetAuditLogsAsync(request.ComId, request.userId);

            return Result<List<AuditLogResponse>>.Success(logs);
        }
    }
}