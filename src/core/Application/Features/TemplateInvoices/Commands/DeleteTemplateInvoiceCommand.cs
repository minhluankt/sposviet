using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.TemplateInvoices.Commands
{
    public class DeleteTemplateInvoiceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteTemplateInvoiceHandler : IRequestHandler<DeleteTemplateInvoiceCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IRepositoryAsync<TemplateInvoice> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly ILogger<DeleteTemplateInvoiceCommand> _log;
            private readonly IHostingEnvironment _hostingEnvironment;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteTemplateInvoiceHandler(IRepositoryAsync<TemplateInvoice> brandRepository,
                ILogger<DeleteTemplateInvoiceCommand> log, IHostingEnvironment hostingEnvironment,
                IFormFileHelperRepository fileHelper,
                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _fileHelper = fileHelper;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _log = log; _hostingEnvironment = hostingEnvironment;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteTemplateInvoiceCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    _log.LogInformation("DeleteTemplateInvoiceCommand start");
                    var product = await _Repository.GetByIdAsync(command.Id);
                    await _Repository.DeleteAsync(product); ;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return await Result<int>.SuccessAsync(product.Id);
                }
                catch (Exception e)
                {
                    _log.LogError("DeleteTemplateInvoiceCommand Exception: " + e.ToString());
                    return await Result<int>.FailAsync(e.Message);
                }

            }
        }
    }
}
