using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using BankService.Model;
using BankService.VietQR;
using Domain.Entities;
using Library;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.VietQRs.Commands
{
 
    public partial class UpdateVietQRCommand : VietQR, IRequest<Result<VietQR>>
    {
        public UpdateVietQRCommand() { }
    }
    public class UpdateVietQRHandler : IRequestHandler<UpdateVietQRCommand, Result<VietQR>>
    {

        private readonly IVietQRRepository<VietQR> _vietQRrepository;
        private readonly IVietQRService _vietQRService;
        public UpdateVietQRHandler(IVietQRService vietQRServicerepository, IVietQRRepository<VietQR> vietQRrepository)
        {
            _vietQRService = vietQRServicerepository;
            _vietQRrepository = vietQRrepository;
        }
        public async Task<Result<VietQR>> Handle(UpdateVietQRCommand request, CancellationToken cancellationToken)
        {
            if (request.Id==0)
            {
                return await _vietQRrepository.AddAsync(request);
            }
            return await _vietQRrepository.UpdateAsync(request);
        }
    }
}
