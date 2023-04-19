using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;

using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Areas.Commands
{

    public class DeleteAreaCommand : IRequest<Result<int>>
    {
        public DeleteAreaCommand(int _comId, int _id)
        {
            ComId = _comId;
            Id = _id;
        }
        public int ComId { get; set; }
        public int Id { get; set; }
        public class DeleteAreaHandler : IRequestHandler<DeleteAreaCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IRepositoryAsync<Area> _Repository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IRepositoryAsync<District> _DistrictRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteAreaHandler(IRepositoryAsync<Product> ProductRepository,
                IRepositoryAsync<District> DistrictRepository,
                IRepositoryAsync<Area> brandRepository,
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
            public async Task<Result<int>> Handle(DeleteAreaCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                if (product != null)
                {
                    await _Repository.DeleteAsync(product);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
