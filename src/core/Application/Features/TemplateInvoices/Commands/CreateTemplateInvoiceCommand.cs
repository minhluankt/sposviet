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

namespace Application.Features.TemplateInvoices.Commands
{

    public partial class CreateTemplateInvoiceCommand : TemplateInvoice, IRequest<Result<int>>
    {

    }
    public class CreateTemplateInvoiceHandler : IRequestHandler<CreateTemplateInvoiceCommand, Result<int>>
    {
        private readonly ILogger<CreateTemplateInvoiceCommand> _log;
        private readonly ITemplateInvoiceRepository<TemplateInvoice> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateTemplateInvoiceHandler(ITemplateInvoiceRepository<TemplateInvoice> brandRepository,
            ILogger<CreateTemplateInvoiceCommand> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<int>> Handle(CreateTemplateInvoiceCommand request, CancellationToken cancellationToken)
        {

            try
            {
                request.Slug = Common.ConvertToSlug(request.Name);

                var fidnmail = _Repository.Entities.AsNoTracking().Where(m => m.Slug == request.Slug && m.ComId == request.ComId).SingleOrDefault();
                if (fidnmail != null)
                {
                    _log.LogError("CreateTemplateInvoiceCommand Create: Đã tồn tại email " + request.Name);
                    return await Result<int>.FailAsync("Đã tồn tại mẫu");
                }


                var product = _mapper.Map<TemplateInvoice>(request);
                await _Repository.AddAsync(product);
                _log.LogInformation("CreateTemplateInvoiceCommand  end " + request.Name);
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
                _log.LogError("CreateTemplateInvoiceCommand Create " + request.Name + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
