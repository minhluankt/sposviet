using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CompanyInfo.Commands
{

    public class DeleteCompanyCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteCompanyHandler : IRequestHandler<DeleteCompanyCommand, Result<int>>
        {
            private readonly IRepositoryAsync<CompanyAdminInfo> _Repository;
            private readonly IRepositoryAsync<Customer> _CustomerRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteCompanyHandler(
                IRepositoryAsync<Customer> CustomerRepository,
                IRepositoryAsync<CompanyAdminInfo> CompanyRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _CustomerRepository = CustomerRepository;
                _Repository = CompanyRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
            {
                await _unitOfWork.CreateTransactionAsync();
                try
                {
                    var product = await _Repository.GetByIdAsync(command.Id);
                    if (product != null)
                    {
                        //var check = _CustomerRepository.GetAll(m => m.i == command.Id).Count();

                        await _Repository.DeleteAsync(product);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _unitOfWork.CommitAsync();
                        return Result<int>.Success((int)product.IdDichVu);
                    }
                    return Result<int>.Fail(HeperConstantss.ERR012);
                }
                catch (System.Exception e)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result<int>.Fail(e.Message);
                }

            }
        }
    }
}
