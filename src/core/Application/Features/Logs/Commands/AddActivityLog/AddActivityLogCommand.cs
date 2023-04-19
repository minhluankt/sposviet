using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ActivityLog.Commands.AddLog
{
    public partial class AddActivityLogCommand : IRequest<Result<int>>
    {
        public string NewValues { get; set; }
        public string OldValues { get; set; }
        public string Action { get; set; }
        public int ComId { get; set; }
        public string userId { get; set; }
    }

    public class AddActivityLogCommandHandler : IRequestHandler<AddActivityLogCommand, Result<int>>
    {
        private readonly ILogRepository _repo;

        private IUnitOfWork _unitOfWork { get; set; }

        public AddActivityLogCommandHandler(ILogRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(AddActivityLogCommand request, CancellationToken cancellationToken)
        {
            await _repo.AddLogAsync(request.ComId,request.Action, request.userId,request.NewValues,request.OldValues);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(1);
        }
    }
}