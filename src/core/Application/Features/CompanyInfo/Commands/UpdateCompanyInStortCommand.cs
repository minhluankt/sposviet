using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CompanyInfo.Commands
{
 
    public class UpdateCompanyInStortCommand : IRequest<Result<CompanyAdminInfo>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string CusTaxCode { get; set; }
        public string Title { get; set; }
        public class UpdateCompanyInStortHandler : IRequestHandler<UpdateCompanyInStortCommand, Result<CompanyAdminInfo>>
        {
            private readonly IRepositoryAsync<CompanyAdminInfo> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public UpdateCompanyInStortHandler(
                IRepositoryAsync<CompanyAdminInfo> CompanyRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
               
                _Repository = CompanyRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<CompanyAdminInfo>> Handle(UpdateCompanyInStortCommand command, CancellationToken cancellationToken)
            {
               
                try
                {
                    var product = await _Repository.GetByIdAsync(command.Id);
                    if (product != null)
                    {
                        product.Name = command.Name;
                        product.Address = command.Address;
                        product.Email = command.Email;
                        product.Title = command.Title;
                        await _Repository.UpdateAsync(product);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        return Result<CompanyAdminInfo>.Success(product);
                    }
                    return Result<CompanyAdminInfo>.Fail(HeperConstantss.ERR012);
                }
                catch (System.Exception e)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result<CompanyAdminInfo>.Fail(e.Message);
                }

            }
        }
    }
}
