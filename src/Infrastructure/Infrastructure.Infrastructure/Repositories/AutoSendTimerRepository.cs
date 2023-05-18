using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spire.Doc.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Infrastructure.Repositories
{
    public class AutoSendTimerRepository : IAutoSendTimerRepository<AutoSendTimer>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private UserManager<ApplicationUser> _userManager;
        private readonly IRepositoryAsync<AutoSendTimer> _repository;
        private IUnitOfWork _unitOfWork { get; set; }
        public AutoSendTimerRepository(IUnitOfWork unitOfWork, IOptions<CryptoEngine.Secrets> config,
            UserManager<ApplicationUser> userManager,
            IRepositoryAsync<AutoSendTimer> repository)
        {
            _config = config;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public IQueryable<AutoSendTimer> Entities => _repository.Entities;

        public async Task<AutoSendTimer> GetById(int id, int ComId)
        {
            return await _repository.Entities.SingleOrDefaultAsync(x=>x.Id==id&&x.ComId==ComId);
        } 
        public async Task<PaginatedList<AutoSendTimer>> GetPageList(int ComId, string sortColumn, string sortColumnDirection, int Currentpage, int pageSite, ENumSupplierEInvoice eNumSupplierEInvoice)
        {
            var datalist =  _repository.Entities.Where(x => x.ComId == ComId).AsNoTracking();
            if (eNumSupplierEInvoice!=ENumSupplierEInvoice.NONE)
            {
                datalist = datalist.Where(x=>x.TypeSupplierEInvoice==eNumSupplierEInvoice);
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            else
            {
                datalist = datalist.OrderByDescending(x => x.Id);
            }
            var data = await PaginatedList<AutoSendTimer>.ToPagedListAsync(datalist, Currentpage, pageSite);
            data.Items.ForEach(x =>
            {
                if (x.CreatedBy != null)
                {
                    x.CreatedBy = _userManager.FindByIdAsync(x.CreatedBy).Result?.FullName;
                }
                var values = "id=" + x.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                x.secret = secret;
            });
            return data;
        }
    }
}
