using Application.Constants;
using Application.Enums;
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

namespace Application.Features.Supplierss.Commands
{
    public partial class CreateSupplierCommand : Suppliers, IRequest<Result<int>>
    {
        public CreateSupplierCommand(int _comId)
        {
            ComId = _comId;
        }
    }
    public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<Suppliers> _Repository;
        private readonly IManagerInvNoRepository _SuppliersRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateSupplierHandler(IRepositoryAsync<Suppliers> brandRepository,
             IFormFileHelperRepository fileHelper, IManagerInvNoRepository SuppliersRepository,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _SuppliersRepository = SuppliersRepository;
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            Suppliers roomAnds = new Suppliers();
            var map = _mapper.Map<Suppliers>(request);
            map.Slug = Common.ConvertToSlug(request.Name);
            int no = await _SuppliersRepository.UpdateInvNo(request.ComId, ENumTypeManagerInv.Suppliers, false);
            map.CodeSupplier = $"SUP{no.ToString("00000")}";
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
