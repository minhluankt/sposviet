using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using BankService.Model;
using BankService.VietQR;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.VietQRs.Query
{
    public class GetAllVietQRQuery : IRequest<Result<List<VietQR>>>
    {
        public int ComId { get; set; }
        public GetAllVietQRQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetAllVietQRCachedQueryHandler : IRequestHandler<GetAllVietQRQuery, Result<List<VietQR>>>
    {
        private readonly IOptions<CryptoEngine.Secrets> _config;
        private readonly IVietQRService _vietQRServicerepository;
        private readonly IVietQRRepository<VietQR> _repository;
        private readonly IMapper _mapper;

        public GetAllVietQRCachedQueryHandler(
            IVietQRRepository<VietQR> repository,
            IMapper mapper,
            IVietQRService vietQRServicerepository,
            IOptions<CryptoEngine.Secrets> config)
        {
            _repository = repository;
            _mapper = mapper;
            _vietQRServicerepository = vietQRServicerepository;
            _config = config;
        }

        public async Task<Result<List<VietQR>>> Handle(GetAllVietQRQuery request, CancellationToken cancellationToken)
        {
            var VietQRs = await _repository.GetAllAsync(request.ComId);
            foreach (var item in VietQRs)
            {
                var values = "id=" + item.Id;
                item.secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                InfoPayQrcode infoPayQrcode = new InfoPayQrcode()
                {
                    accountName = item.BankAccount.BankName,
                    accountNo = item.BankAccount.BankNumber,
                    acqId = item.BankAccount.BinVietQR,
                    template = GeneralMess.ConvertEnumTemplateVietQR(EnumTemplateVietQR.compact2),
                };

                var callapi = await _vietQRServicerepository.GetQRCode(infoPayQrcode);
                if (callapi != null)
                {

                }
            }
            return Result<List<VietQR>>.Success(VietQRs);
        }
    }
}
