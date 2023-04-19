using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Supplierss.Commands
{
    public partial class UpdateSuppliersCommand : Suppliers, IRequest<Result<int>>
    {
        public UpdateSuppliersCommand(int _comId)
        {
            ComId = _comId;
        }
        public IFormFile Img { get; set; }
    }
    public class UpdateSuppliersHandler : IRequestHandler<UpdateSuppliersCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Suppliers> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateSuppliersHandler(IRepositoryAsync<Suppliers> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateSuppliersCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.Slug = Common.ConvertToSlug(command.Name);
                brand.Name = command.Name;
                brand.Address = command.Address;
                brand.TaxCode = command.TaxCode;
                brand.Email = command.Email;
                brand.Phonenumber = command.Phonenumber;
                var checkcode = _Repository.Entities.Count(predicate: m => m.Slug == brand.Slug && m.Id != brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
