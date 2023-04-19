using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class SupplierEInvoiceRepository : ISupplierEInvoiceRepository<SupplierEInvoice>
    {
        private readonly IRepositoryAsync<SupplierEInvoice> _repository;
        public SupplierEInvoiceRepository(IRepositoryAsync<SupplierEInvoice> repository) {
            _repository = repository;
        }

        public IQueryable<SupplierEInvoice> Entities  => _repository.Entities;
        public async Task AddAsync(SupplierEInvoice entity)
        {
             await _repository.AddAsync(entity);
        }

        public async Task<IResult> DeleteAsync(int Id, int Comid)
        {
            var getdata = await _repository.Entities.SingleOrDefaultAsync(x => x.Id == Id&&x.ComId==Comid);
            if (getdata!=null)
            {
                await _repository.DeleteAsync(getdata);
                return await Result.SuccessAsync();
            }
            return await Result.FailAsync();
        }

     

        public async Task<SupplierEInvoice> GetByIdAsync(int Comid,ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            //return await _repository.GetByIdAsync(x=>x.TypeSupplierEInvoice== TypeSupplierEInvoice, x=>x.Include(x=>x.ManagerPatternEInvoices));
            return await _repository.Entities.SingleOrDefaultAsync(x=>x.TypeSupplierEInvoice== TypeSupplierEInvoice && x.ComId== Comid);
        }

        public async Task<IResult> UpdateAsync(SupplierEInvoice Entity, int Comid)
        {
            var getdata = await _repository.SingleByExpressionAsync(x => x.Id == Entity.Id && x.ComId == Comid);
            if (getdata != null)
            {
                getdata.DomainName= Entity.DomainName;
                getdata.UserNameService= Entity.UserNameService;
                getdata.PassWordService= Entity.PassWordService;
                getdata.UserNameAdmin= Entity.UserNameAdmin;
                getdata.PassWordAdmin= Entity.PassWordAdmin;
                getdata.Active= Entity.Active;
                getdata.SerialCert= Entity.SerialCert;
                getdata.TypeSeri= Entity.TypeSeri;
                await _repository.UpdateAsync(getdata);
                return await Result.SuccessAsync();
            }
            return await Result.FailAsync();
        }
    }
}
