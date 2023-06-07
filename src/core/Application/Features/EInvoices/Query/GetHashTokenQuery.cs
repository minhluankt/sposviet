using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.ViewModel;
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
                    var getlist = await _repository.Entities.Where(x=> query.lstid.Contains(x.Id)).Include(x=>x.EInvoiceItems).ToListAsync();
                    var checkinv = getlist.Select(x=> x.TypeSupplierEInvoice).Distinct().ToList();
                    if (checkinv.Count()>1|| checkinv.Count()==0)
                    {
                        return await Result<HashTokenModel>.FailAsync(HeperConstantss.ERR052);
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
