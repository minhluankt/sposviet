using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using BankService.Model;
using BankService.VietQR;
using Domain.Entities;
using Library;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
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

        private readonly IVietQRService _vietQRServicerepository;
        public GenerateVietQRHandler(IVietQRService vietQRServicerepository)
        {
            _vietQRServicerepository = vietQRServicerepository;
        }


        public async Task<Result<VietQRData>> Handle(GenerateVietQRCommand request, CancellationToken cancellationToken)
        {
            var callapi = await _vietQRServicerepository.GetQRCode(request.infoPayQrcode);
            if (!callapi.isError)
            {
                var data = ConvertSupport.ConverJsonToModel<VietQRData>(callapi.data);

                return Result<VietQRData>.Success(data,HeperConstantss.SUS006);
            }
            return Result<VietQRData>.Fail();
        }
    }
}
