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

namespace Application.Features.Kitchens.Commands
{
    public partial class CreateNotifyChitkenCommand : NotifyKitChenModel, IRequest<Result<string>>
    {

    }
    public class CreateNotifyChitkenHandler : IRequestHandler<CreateNotifyChitkenCommand, Result<string>>
    {
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
            IRepositoryAsync<Customer> customerRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _log = log;
            _companyProductRepository = companyProductRepository;
            _NotifyChitkenRepository = NotifyChitkenRepository;
            _roomAndTableRepository = roomAndTableRepository;
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
                        string html = "<!DOCTYPE html>\r\n<html lang='vi'>\r\n<head>\r\n    <meta charset='UTF-8'>\r\n    <meta name='viewport' content='width=device-width, initial-scale=1.0'>\r\n    <meta http-equiv='X-UA-Compatible' content='ie=edge'>\r\n    <title>Vé điện tử</title>\r\n    <script type=\"text/javascript\" charset=\"UTF-8\"></script>\r\n\t\r\n\t<style>\r\n        body {\r\n           \r\n            font-family: Arial;\r\n        }\r\n\r\n        hr {\r\n            margin: 0px;\r\n            border-top: 1px solit #000;\r\n        }\r\n\r\n        .ticket {\r\n          \r\n            padding: 0mm;\r\n            margin: 0 auto;\r\n            height: auto;\r\n   width: 300mm;\r\n            background: #FFF;\r\n            transform-origin: left;\r\n        }\r\ntable { \r\n    border-collapse: collapse; \r\n}\r\ntable td,table th{\r\npadding:2px 2px 2px 0px;\r\n}\r\n        img {\r\n            max-width: inherit;\r\n            width: inherit;\r\n        }\r\n\r\n        @media print {\r\n\r\n            .hidden-print,\r\n            .hidden-print * {\r\n                display: none !important;\r\n            }\r\n\r\n            .ticket {\r\n                page-break-after: always;\r\n            }\r\n        }\r\n    </style>\r\n</head>\r\n\r\n<body>\r\n    <div class='ticket'>\r\n\t\r\n        <table style='width:100%;'>\r\n            <tr>\r\n                <td style='text-align: center;'>\r\n\t\t\t\t\t<span style='font-weight: bold;font-size: 50pt;'>{comname}</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block; text-align: center;margin-bottom:10px'>----------***----------</span>\r\n\t\t\t\t\t\r\n\t\t\t\t</td>\r\n            </tr>\r\n            <tr>\r\n                <td style='font-size: 18px; text-align: center; padding-top: 7px; padding-bottom: 7px;'>\r\n\t\t\t\t\t<span style='display: block; font-size: 45pt; font-weight: bold;'>THÔNG BÁO CHẾ BIẾN</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block;'>Thời gian: {ngaythangnamxuat}</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block;'>Nhân viên phục vụ: {staffName}</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block;'>Bàn: {tenbanphong}</span>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n\t\r\n        \r\n\t\t<hr style=\"font-size:40pt\" />\r\n\t<table style='width:100%;margin-top:20pt;margin-bottom:10px'>\t\t\t\r\n\t    <thead>\r\n\t\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t<th style='font-size: 35pt; text-align: left;    PADDING-BOTTOM: 12PT;'>Tên hàng hóa</th>\r\n\t\t<th style='font-size: 35pt; text-align: right;    PADDING-BOTTOM: 12PT;'>Số lượng</th>\r\n\t\t</tr>\r\n\t\t</thead>\r\n\t\t<tbody>\r\n\t\t<tr>\r\n\t\t<td style=\" padding-top: 10pt;padding-bottom: 10pt;\"colspan=\"2\">\r\n\t\t\t<span style='border-top: dotted #000 4px;display: block;'></span>\r\n\t\t</td>\r\n\t\t</tr>\r\n\t\t\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t\t\t<td style='font-size: 40pt; text-align: left'>{tenhanghoa}</td>\r\n\t\t\t\t<td style='font-size: 40pt; text-align: right'>{soluong}</td>\r\n\t\t\t</tr>\r\n\r\n\t\t</tbody>\r\n\t\t<tfoot>\r\n\t\t<tr><td style=\" padding-top: 10pt;padding-bottom: 10pt;\"colspan=\"2\">\r\n\t\t\t<span style='border-top: dotted #000 4px;display: block;'></span>\r\n\t\t</td></tr>\r\n\t\t<tr style='font-size: 35pt;text-align: left;margin-top:4px;border-top-style: dotted;border-width: 0.1px;'>\r\n\t\t\t<td style='font-size: 50pt; text-align: left'>Tổng</td>\r\n\t\t\t<td style=\"text-align: right;font-size: 50pt;\">{tongsoluong}</td>\r\n\t\t</tr>\r\n\t\t\r\n\t\t</tfoot>\r\n\t</table>\r\n\t\r\n</body>\r\n</html>";
                        string trproductregex = @"<tbody>(?<xValue>(.|\n)*)<\/tbody>";
                        CompanyAdminInfo company = _companyProductRepository.GetCompany(request.ComId);
                       // TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(command.ComId);
                        //if (templateInvoice != null)
                        //{
                            TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                            {
                                comname = !string.IsNullOrEmpty(company.Title) ? company.Title.Trim() : company.Name,
                                ngaythangnamxuat = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                tongsoluong = add.Data.Sum(x=>x.Quantity).ToString("N0"),
                                tenbanphong = add.Data.FirstOrDefault()?.RoomTableName,
                                staffName = request.Cashername,
                            };
                            
                            string tableProduct = string.Empty;
                            Regex rg = new Regex(trproductregex);
                            var match = rg.Match(html);
                            String result = match.Groups["xValue"].Value;
                            if (!string.IsNullOrEmpty(result))
                            {
                                foreach (var item in add.Data)
                                {
                                    tableProduct += result.Replace("{tenhanghoa}", item.Name).Replace("{soluong}", item.Quantity.ToString("N0"));
                                }
                            }
                            html = html.Replace(result, tableProduct);
                            string content = LibraryCommon.GetTemplate(templateInvoiceParameter, html, EnumTypeTemplate.PRINT_BEP);
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
