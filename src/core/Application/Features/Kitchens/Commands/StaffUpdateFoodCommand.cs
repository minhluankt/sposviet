using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Kitchens.Commands
{

    public partial class StaffUpdateFoodCommand : NotifyKitChenModel, IRequest<Result<List<Kitchen>>>
    {
        public string Barname { get;set; }//nhân viên bếp
    }
    public class StaffUpdateFoodHandler : IRequestHandler<StaffUpdateFoodCommand, Result<List<Kitchen>>>
    {
        private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
        private readonly ICompanyAdminInfoRepository _companyProductRepository;
        private readonly ILogger<StaffUpdateFoodHandler> _log;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly INotifyChitkenRepository _NotifyChitkenRepository;
        private readonly IRepositoryAsync<Customer> _customerRepository;
        private readonly IRepositoryAsync<RoomAndTable> _roomAndTableRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public StaffUpdateFoodHandler(
            ILogger<StaffUpdateFoodHandler> log, ICompanyAdminInfoRepository companyProductRepository,
            INotifyChitkenRepository NotifyChitkenRepository,
            IRepositoryAsync<RoomAndTable> roomAndTableRepository,
            IRepositoryAsync<Customer> customerRepository, ITemplateInvoiceRepository<TemplateInvoice> templateInvoicerepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _log = log;
            _companyProductRepository = companyProductRepository;
            _NotifyChitkenRepository = NotifyChitkenRepository;
            _roomAndTableRepository = roomAndTableRepository;
            _templateInvoicerepository = templateInvoicerepository;
            _customerRepository = customerRepository;
            _fileHelper = fileHelper;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<List<Kitchen>>> Handle(StaffUpdateFoodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.lstIdChiken==null || request.lstIdChiken.Count()==0)
                {
                    return Result<List<Kitchen>>.Fail("Vui lòng chọn món");
                }
                var add = await _NotifyChitkenRepository.StaffUpdateStaus(request.ComId, request.lstIdChiken, request.Cashername, request.Barname,request.IsCancel);
                if (add.Succeeded)
                {
                    return await Result<List<Kitchen>>.SuccessAsync(add.Data,HeperConstantss.SUS006);
                }
                return Result<List<Kitchen>>.Fail(add.Message);
            }
            catch (System.Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<List<Kitchen>>.FailAsync(e.Message);
            }
        }
    }
}
