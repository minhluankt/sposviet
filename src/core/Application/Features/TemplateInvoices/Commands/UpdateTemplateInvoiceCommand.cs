using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.TemplateInvoices.Commands
{

    public partial class UpdateTemplateInvoiceCommand : TemplateInvoice, IRequest<Result<int>>
    {
    }
    public class UpdateTemplateInvoiceHandler : IRequestHandler<UpdateTemplateInvoiceCommand, Result<int>>
    {
        private readonly ILogger<UpdateTemplateInvoiceHandler> _log;
        private readonly ITemplateInvoiceRepository<TemplateInvoice> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateTemplateInvoiceHandler(ITemplateInvoiceRepository<TemplateInvoice> brandRepository,
            ILogger<UpdateTemplateInvoiceHandler> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }


        public async Task<Result<int>> Handle(UpdateTemplateInvoiceCommand command, CancellationToken cancellationToken)
        {
            try
            {
                command.Slug = Common.ConvertToSlug(command.Name);

                var fidnmail = _Repository.Entities.Where(m => m.Slug == command.Slug && m.ComId == command.ComId && m.Id != command.Id).SingleOrDefault();
                if (fidnmail != null)
                {
                    _log.LogError("UpdateTemplateInvoiceCommand Không tìm thấy mẫu  " + command.Name);
                    return await Result<int>.FailAsync("Không tìm thấy mẫu");
                }
                var product = _mapper.Map<TemplateInvoice>(command);
                await _Repository.UpdateAsync(product);
                _log.LogInformation("UpdateTemplateInvoiceCommand  end " + command.Name);
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
                _log.LogError("UpdateTemplateInvoiceCommand Create " + command.Name + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
