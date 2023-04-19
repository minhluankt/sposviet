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

namespace Application.Features.Areas.Commands
{
    public partial class CreateAreaCommand : Area, IRequest<Result<int>>
    {
        public CreateAreaCommand(int _comId)
        {
            ComId = _comId;
        }
    }
    public class CreateAreaHandler : IRequestHandler<CreateAreaCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<Area> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateAreaHandler(IRepositoryAsync<Area> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
        {
            Area roomAnds = new Area();
            var map = _mapper.Map<Area>(request);
            map.Slug = Common.ConvertToSlug(request.Name);
            var fidn = _Repository.Entities.Where(m => m.ComId == request.ComId && m.Slug == map.Slug).FirstOrDefault();
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
