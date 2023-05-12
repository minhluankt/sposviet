using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SystemVariable;
using System.Linq;
using Application.Hepers;

namespace Application.Features.Kitchens.Commands
{
    public partial class CreateNotifyChitkenCommand : NotifyKitChenModel, IRequest<Result<string>>
    {

    }
    public class CreateNotifyChitkenHandler : IRequestHandler<CreateNotifyChitkenCommand, Result<string>>
    {
        private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
        private readonly ICompanyAdminInfoRepository _companyProductRepository;
        private readonly ILogger<CreateNotifyChitkenHandler> _log;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly INotifyChitkenRepository _NotifyChitkenRepository;
        private readonly IRepositoryAsync<Customer> _customerRepository;
        private readonly IRepositoryAsync<RoomAndTable> _roomAndTableRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }
        
        public CreateNotifyChitkenHandler(
            ILogger<CreateNotifyChitkenHandler> log, ICompanyAdminInfoRepository companyProductRepository,
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

        public async Task<Result<string>> Handle(CreateNotifyChitkenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.IdOrder == null)
                {
                    return await Result<string>.FailAsync("Không tìm thấy đơn");
                }
                var add=  await _NotifyChitkenRepository.NotifyOrder(request.ComId, request.IdOrder.Value,request.Cashername);
                if (add.Succeeded)
                {
                    if (add.Data.Count>0)
                    {
                        TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(request.ComId, EnumTypeTemplatePrint.IN_BA0_CHE_BIEN);
                        if (templateInvoice == null)
                        {
                            return await Result<string>.SuccessAsync("ERR","Không tìm thấy mẫu in báo chế biến, không thể in");
                        }
                        string html = templateInvoice.Template;
                        if (string.IsNullOrEmpty(html))
                        {
                            return await Result<string>.SuccessAsync("ERR", "Mẫu in không có dữ liệu vui lòng kiêm tra lại mẫu in báo chế biến");
                        }
                        CompanyAdminInfo company = _companyProductRepository.GetCompany(request.ComId);
                        TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                        {
                            comname = !string.IsNullOrEmpty(company.Title) ? company.Title.Trim() : company.Name,
                            ngaythangnamxuat = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            tongsoluong = add.Data.Sum(x => x.Quantity).ToString("N0"),
                            tenbanphong = add.Data.FirstOrDefault()?.RoomTableName,
                            staffName = request.Cashername,
                        };
                        string content = PrintTemplate.PrintBaoBep(templateInvoiceParameter, add.Data, templateInvoice.Template);
                        return Result<string>.Success(content, add.Message);

                    }
                    else
                    {
                        return Result<string>.Success("Không có đơn để in", add.Message);
                    }
                }
                return Result<string>.Fail(add.Message);
            }
            catch (System.Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<string>.FailAsync(e.Message);
            }
        }
    }
}
