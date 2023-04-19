using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Units.Commands
{
    public partial class CreateUnitCommand : Domain.Entities.Unit, IRequest<Result<int>>
    {
        public CreateUnitCommand(int _comId)
        {
            ComId = _comId;
        }
    }
    public class CreateUnitHandler : IRequestHandler<CreateUnitCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<Domain.Entities.Unit> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateUnitHandler(IRepositoryAsync<Domain.Entities.Unit> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Unit roomAnds = new Domain.Entities.Unit();
            var map = _mapper.Map<Domain.Entities.Unit>(request);
            map.Code = Common.ConvertToSlug(request.Name);
            if (string.IsNullOrEmpty(map.FullName))
            {
                map.FullName = map.Name;
            }
            var fidn = _Repository.Entities.Where(m => m.ComId == request.ComId && m.Code == map.Code).FirstOrDefault();
            if (fidn != null)
            {
                return Result<int>.Fail(HeperConstantss.ERR014);
            }
            await _Repository.AddAsync(map);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success();
        }
    }
}
