using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using BankService.Model;
using BankService.VietQR;
using CoreHtmlToImage;
using HelperLibrary;
using Library;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
            if (request.infoPayQrcode.template == EnumTemplateVietQR.compact2.ToString() && string.IsNullOrEmpty(request.infoPayQrcode.amount))
            {
                request.infoPayQrcode.amount = "0";
            }
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
                    //int start = 11;
                    //StringBuilder sb = new StringBuilder(data.qrCode);
                    //sb
                    //  .Remove(start, length)
                    //  .Insert(start, replacementText);
                    data.qrCode = data.qrCode.ReplaceAt(10,2,"11");//12 là chỉ quét 1 lần
                    string path = _iFormFileHelperRepository.GetFileTemplate(FileConstants.logAppsposviet,string.Empty, FolderUploadConstants.Images);
                    int widthqrcode = 0;
                    Bitmap image1 = null;
                    if (!string.IsNullOrEmpty(path))
                    {
                       image1 = (Bitmap)Image.FromFile(path, true);
                    }
                    string qrcodedata = ConvertSupport.ConverStringToQrcode(data.qrCode, 20, image1,20);

                    var listbank = Common.ConverJsonToModel<List<BankAccountModel>>(getjsonbank);
                    var bank = listbank.SingleOrDefault(x => x.bin == request.infoPayQrcode.acqId);
                    gettem = gettem.Replace("{qrcodedata}", qrcodedata);
                    if (request.infoPayQrcode.template== EnumTemplateVietQR.print.ToString())
                    {
                        widthqrcode = 600;
                        gettem = gettem.Replace("{tenchutaikhoan}", request.infoPayQrcode.accountName).
                                        Replace("{sotaikhoan}", request.infoPayQrcode.accountNo).
                                        Replace("{tennganhang}", bank.name)
                                        .Replace("{logobank}", bank.logo);
                    }
                    else if (request.infoPayQrcode.template == EnumTemplateVietQR.compact.ToString())
                    {
                        widthqrcode = 540;//540x540
                        gettem = gettem.Replace("{logobank}", bank.logo);
                    } 
                    else if (request.infoPayQrcode.template == EnumTemplateVietQR.compact2.ToString())
                    {
                        widthqrcode = 540;//540x640
                        gettem = gettem.Replace("{logobank}", bank.logo).
                                        Replace("{tenchutaikhoan}", request.infoPayQrcode.accountName).
                                       Replace("{sotaikhoan}", request.infoPayQrcode.accountNo).
                                       Replace("{sotien}", request.infoPayQrcode.amount);
                    }
                    else if (request.infoPayQrcode.template == EnumTemplateVietQR.qr_only.ToString())
                    {
                        widthqrcode = 480;//480x480
                    }
                    var converter = new HtmlConverter();
                    var Base64String = $"data:image/jpg;base64,{Convert.ToBase64String(converter.FromHtmlString(gettem, widthqrcode))}";
                    

                    data.qrDataURL = Base64String;
                }
                return Result<VietQRData>.Success(data,HeperConstantss.SUS006);
            }
            return Result<VietQRData>.Fail();
        }
    }
}
