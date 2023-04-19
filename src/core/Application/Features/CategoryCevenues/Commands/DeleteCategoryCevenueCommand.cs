using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategoryCevenues.Commands
{

    public class DeleteCategoryCevenueCommand : IRequest<Result<int>>
    {
        public DeleteCategoryCevenueCommand(int _comId, int _id)
        {
            ComId = _comId;
            Id = _id;
        }
        public int ComId { get; set; }
        public int Id { get; set; }
        public class DeleteCategoryCevenueHandler : IRequestHandler<DeleteCategoryCevenueCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IRepositoryAsync<CategoryCevenue> _Repository;
            private readonly IRepositoryAsync<RevenueExpenditure> _ProductRepository;
            private readonly IRepositoryAsync<District> _DistrictRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteCategoryCevenueHandler(IRepositoryAsync<RevenueExpenditure> ProductRepository,
                IRepositoryAsync<District> DistrictRepository,
                IRepositoryAsync<CategoryCevenue> brandRepository,
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
            public async Task<Result<int>> Handle(DeleteCategoryCevenueCommand command, CancellationToken cancellationToken)
            {
                var cehck = await _ProductRepository.Entities.FirstOrDefaultAsync(x=>x.IdCategoryCevenue== command.Id);
                if (cehck != null)
                {
                    return Result<int>.Fail(HeperConstantss.ERR016);
                }
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
