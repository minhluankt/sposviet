using Application.Constants;
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

    public class DeleteUnitCommand : IRequest<Result<int>>
    {
        public DeleteUnitCommand(int _comId, int _id)
        {
            ComId = _comId;
            Id = _id;
        }
        public int ComId { get; set; }
        public int Id { get; set; }
        public class DeleteUnitHandler : IRequestHandler<DeleteUnitCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IRepositoryAsync<Domain.Entities.Unit> _Repository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IRepositoryAsync<District> _DistrictRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteUnitHandler(IRepositoryAsync<Product> ProductRepository,
                IRepositoryAsync<District> DistrictRepository,
                IRepositoryAsync<Domain.Entities.Unit> brandRepository,
                   IFormFileHelperRepository fileHelper,
                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _DistrictRepository = DistrictRepository;
                _ProductRepository = ProductRepository;
                _fileHelper = fileHelper;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteUnitCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                if (product != null)
                {
                    var checkpro = _ProductRepository.Entities.Count(x => x.IdUnit == command.Id);
                    if (checkpro>0)
                    {
                        return Result<int>.Fail("Đơn vị tính đã được sử dụng, không thể xóa");
                    }
                    await _Repository.DeleteAsync(product);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
