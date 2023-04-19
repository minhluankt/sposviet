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

namespace Application.Features.CategoryCevenues.Commands
{
    public partial class CreateCategoryCevenueCommand : CategoryCevenue, IRequest<Result<int>>
    {
        public CreateCategoryCevenueCommand(int _comId)
        {
            ComId = _comId;
        }
    }
    public class CreateCategoryCevenueHandler : IRequestHandler<CreateCategoryCevenueCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<CategoryCevenue> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateCategoryCevenueHandler(IRepositoryAsync<CategoryCevenue> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateCategoryCevenueCommand request, CancellationToken cancellationToken)
        {
            if (request.ComId <=0)
            {
                return Result<int>.Fail(HeperConstantss.ERR000);
            }
            CategoryCevenue roomAnds = new CategoryCevenue();
            var map = _mapper.Map<CategoryCevenue>(request);
            map.Slug = Common.ConvertToSlug($"{request.Name}-{request.Type}");
            map.Code = Common.RandomString(8);
            var fidn = _Repository.Entities.Where(m => m.ComId == request.ComId && m.Slug == map.Slug&&m.Type== request.Type).FirstOrDefault();
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
