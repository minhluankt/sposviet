using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.EInvoices.Query
{

    public class GetHashTokenQuery : IRequest<IResult<HashTokenModel>>
    {

        public int[] lstid { get; set; }
        public int ComId { get; set; }
        public bool IsGetHashPublish { get; set; }

        public class GetHashTokenQueryHandler : IRequestHandler<GetHashTokenQuery, IResult<HashTokenModel>>
        {
            private readonly IRepositoryAsync<Domain.Entities.EInvoice> _repository;
            private readonly ISupplierEInvoiceRepository<Domain.Entities.SupplierEInvoice> _supplierEInvoicerepository;
            private readonly IEInvoiceRepository<Domain.Entities.EInvoice> _EInvoicerepository;
            public GetHashTokenQueryHandler(IRepositoryAsync<Domain.Entities.EInvoice> repository,
                ISupplierEInvoiceRepository<Domain.Entities.SupplierEInvoice> supplierEInvoicerepository,
                IEInvoiceRepository<Domain.Entities.EInvoice> EInvoicerepository)
            {
                _EInvoicerepository = EInvoicerepository;
                _repository = repository;
                _supplierEInvoicerepository = supplierEInvoicerepository;
            }
            public async Task<IResult<HashTokenModel>> Handle(GetHashTokenQuery query, CancellationToken cancellationToken)
            {
                HashTokenModel hashTokenModel = new HashTokenModel();
                if (query.IsGetHashPublish)
                {
                    var getlist = await _repository.Entities.AsNoTracking().Where(x=> query.lstid.Contains(x.Id)).Include(x=>x.EInvoiceItems).ToListAsync();
                    if (getlist.Count()==0)
                    {
                        return await Result<HashTokenModel>.FailAsync(HeperConstantss.ERR012);
                    }
                    var checkinv = getlist.Select(x=> x.TypeSupplierEInvoice).Distinct().ToList();
                    if (checkinv.Count()>1|| checkinv.Count()==0)
                    {
                        return await Result<HashTokenModel>.FailAsync(HeperConstantss.ERR052);
                    }
                    //check trangj thai hóa đơn có đúng k
                    var checkstatus = getlist.Where(x=>x.StatusEinvoice!=Enums.StatusEinvoice.NewInv && x.StatusEinvoice != Enums.StatusEinvoice.Null).ToList();
                    if (checkstatus.Count()>0)
                    {
                        var mes = checkstatus.Select(x=> LibraryCommon.GetDisplayNameEnum(x.StatusEinvoice)).Distinct().ToList();
                        return await Result<HashTokenModel>.FailAsync($"Hóa đơn có các trạng thái sau không thể phát hành!\n {string.Join(',', mes)}");
                    }


                    //check hóa đơn gì nếu mtt thì phát hành luôn.
                    var getserial = getlist.DistinctBy(x => new { x.Pattern ,x.Serial}).ToList();
                    if (getserial.Count() > 1)
                    {
                        if (getserial.Any(x=> LibraryCommon.IsHDDTMayTinhTien(x.Serial)))
                        {
                            return await Result<HashTokenModel>.FailAsync($"Vui lòng chỉ lựa chọn một loại hóa đơn để phát hành");
                        }
                    }
                    else if (getserial.Any(x => LibraryCommon.IsHDDTMayTinhTien(x.Serial)))
                    {
                        //xử  lý phát hành hóa đơn máy tính tiền tại đây, gọi hàm bên einvoice xl
                    }
                    var getsup = await _supplierEInvoicerepository.GetByIdAsync(query.ComId, getlist.FirstOrDefault().TypeSupplierEInvoice);
                    if (getsup==null)
                    {
                        return await Result<HashTokenModel>.FailAsync(HeperConstantss.ERR012);
                    }

                    var getdata = await _EInvoicerepository.GetHashInvWithTokenVNPTAsync(getlist, getsup);
                    if (getdata.Succeeded)
                    {
                        hashTokenModel.dataxmlhash = getdata.Data;
                        hashTokenModel.pattern = getlist.FirstOrDefault().Pattern;
                        hashTokenModel.serial = getlist.FirstOrDefault().Serial;
                        hashTokenModel.TypeSupplierEInvoice = getlist.FirstOrDefault().TypeSupplierEInvoice;
                        hashTokenModel.serialCert = getsup.SerialCert;

                        return await Result<HashTokenModel>.SuccessAsync(hashTokenModel);
                    }
                    return await Result<HashTokenModel>.FailAsync(getdata.Message);
                }
                var getdatavnpt = await _EInvoicerepository.GetHashTokenVNPTAsync(query.lstid, query.ComId);
                hashTokenModel.dataxmlhash = getdatavnpt.Data;
                return await Result<HashTokenModel>.SuccessAsync(hashTokenModel);
            }
        }
    }
}
