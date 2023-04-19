using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ManagerPatternEInvoices.Commands
{

    public partial class CreateManagerPatternEInvoiceCommand : ManagerPatternEInvoice, IRequest<Result<int>>
    {

    }
    public class CreateManagerPatternEInvoiceHandler : IRequestHandler<CreateManagerPatternEInvoiceCommand, Result<int>>
    {
        private readonly ILogger<CreateManagerPatternEInvoiceCommand> _log;
        private readonly IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateManagerPatternEInvoiceHandler(IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> brandRepository,
            ILogger<CreateManagerPatternEInvoiceCommand> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<int>> Handle(CreateManagerPatternEInvoiceCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var product = _mapper.Map<ManagerPatternEInvoice>(request);
                var getkey = await _Repository.GetbykeyAsync(product.VFkey);
                if (getkey != null)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                if (request.Selected)
                {
                    var getall = _Repository.Entities.Where(x => x.ComId == request.ComId).ToList();
                    if (getall.Count()>0)
                    {
                        getall.ForEach(x => x.Selected = false);
                        await _Repository.UpdateRangeAsync(getall);
                        await _unitOfWork.SaveChangesAsync();
                    }
                  
                }
                await _Repository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError("CreateManagerPatternEInvoiceCommand Create " + request.TypeSupplierEInvoice + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
