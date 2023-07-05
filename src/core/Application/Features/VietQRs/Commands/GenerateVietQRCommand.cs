using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using BankService.Model;
using BankService.VietQR;
using CoreHtmlToImage;
using Library;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.VietQRs.Commands
{
    public partial class GenerateVietQRCommand : IRequest<Result<VietQRData>>
    {
        public InfoPayQrcode infoPayQrcode { get; set; }
        public GenerateVietQRCommand() { }
    }
    public class GenerateVietQRHandler : IRequestHandler<GenerateVietQRCommand, Result<VietQRData>>
    {
        private IFormFileHelperRepository _iFormFileHelperRepository;
        private readonly IVietQRService _vietQRServicerepository;
        public GenerateVietQRHandler(IVietQRService vietQRServicerepository, IFormFileHelperRepository iFormFileHelperRepository)
        {
            _vietQRServicerepository = vietQRServicerepository;
            _iFormFileHelperRepository = iFormFileHelperRepository;
        }


        public async Task<Result<VietQRData>> Handle(GenerateVietQRCommand request, CancellationToken cancellationToken)
        {
            var callapi = await _vietQRServicerepository.GetQRCode(request.infoPayQrcode);
            if (!callapi.isError)
            {
                var data = ConvertSupport.ConverJsonToModel<VietQRData>(callapi.data);
                var getjsonbank = _iFormFileHelperRepository.GetContentFile(FileConstants.BankAccountVietQR, FolderUploadConstants.BankAccountFolder);
                if (getjsonbank == "")
                {
                    return Result<VietQRData>.Success(data, HeperConstantss.SUS006);
                }
                string filename = $"vietqr_{request.infoPayQrcode.template}.txt";
                var gettem = _iFormFileHelperRepository.GetContentFile(filename, FolderUploadConstants.VietQR);
                if (!string.IsNullOrEmpty(gettem))
                {
                    string path = _iFormFileHelperRepository.GetFileTemplate(FileConstants.logAppsposviet,string.Empty, FolderUploadConstants.Images);
                   
                    string qrcodedata = string.Empty;
                    if (!string.IsNullOrEmpty(path))
                    {
                        Bitmap image1 = (Bitmap)Image.FromFile(path, true);
                        qrcodedata = ConvertSupport.ConverStringToQrcode(data.qrCode, 20, image1);
                    }
                    else
                    {
                        qrcodedata = ConvertSupport.ConverStringToQrcode(data.qrCode, 20);
                    }

                    var listbank = Common.ConverJsonToModel<List<BankAccountModel>>(getjsonbank);
                    var bank = listbank.SingleOrDefault(x => x.bin == request.infoPayQrcode.acqId);
                    gettem = gettem.Replace("{qrcodedata}", qrcodedata);
                    if (request.infoPayQrcode.template== EnumTemplateVietQR.print.ToString())
                    {
                        gettem = gettem.Replace("{tenchutaikhoan}", request.infoPayQrcode.accountName).
                                        Replace("{sotaikhoan}", request.infoPayQrcode.accountNo).
                                        Replace("{tennganhang}", bank.name)
                                        .Replace("{logobank}", bank.logo);
                    }
                    else if (request.infoPayQrcode.template == EnumTemplateVietQR.compact.ToString())
                    {
                        gettem = gettem.Replace("{logobank}", bank.logo);
                    }
                    else if (request.infoPayQrcode.template == EnumTemplateVietQR.compact2.ToString())
                    {
                        gettem = gettem.Replace("{logobank}", bank.logo).
                                       Replace("{sotaikhoan}", request.infoPayQrcode.accountNo).
                                       Replace("{sotien}", request.infoPayQrcode.amount);
                    }
                  
                    var converter = new HtmlConverter();
                    var Base64String = $"data:image/jpg;base64,{Convert.ToBase64String(converter.FromHtmlString(gettem, 600 ))}";
                    

                    data.qrDataURL = Base64String;
                }
                return Result<VietQRData>.Success(data,HeperConstantss.SUS006);
            }
            return Result<VietQRData>.Fail();
        }
    }
}
