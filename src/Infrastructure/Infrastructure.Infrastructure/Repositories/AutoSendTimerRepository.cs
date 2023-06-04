using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.Identity;
using Hangfire;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NStandard;
using Org.BouncyCastle.Asn1.Ocsp;
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
        private ILogger<AutoSendTimerRepository> _logger;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEInvoiceRepository<EInvoice> _einvoice;
        private readonly IRepositoryAsync<AutoSendTimer> _repository;
        private IUnitOfWork _unitOfWork { get; set; }
        public AutoSendTimerRepository(IUnitOfWork unitOfWork,
            IEInvoiceRepository<EInvoice> einvoice,
            ILogger<AutoSendTimerRepository> logger, IOptions<CryptoEngine.Secrets> config,
            UserManager<ApplicationUser> userManager,
            IRepositoryAsync<AutoSendTimer> repository)
        {
            _config = config;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _einvoice = einvoice;
            _logger = logger;
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
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
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

        public async Task<AutoSendTimer> AddAsync(AutoSendTimer entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity;
        }

        public async Task<Result<AutoSendTimer>> UpdateAsync(AutoSendTimer entity)
        {
            var data = await _repository.Entities.SingleOrDefaultAsync(x => x.Id == entity.Id && x.ComId == entity.ComId);
            if (data != null)
            {
                data.Name = entity.Name;
                data.Hour = entity.Hour;
                data.Minute = entity.Minute;
                string mess = string.Empty;
                if (data.Active && !entity.Active)//nếu dg bật mà update tắt thì xóa
                {
                    RecurringJob.RemoveIfExists(data.JobId.ToString());
                    mess = "Đã hủy bỏ ứng dụng đang chạy";
                }
                else if(!data.Active && entity.Active)//ngược lại
                {
                    RecurringJob.AddOrUpdate(data.JobId.ToString(), () => this.StartJobEInvoiceAsync(data), Cron.Daily(data.Hour, data.Minute), TimeZoneInfo.Local);
                    _logger.LogInformation("Kích hoạt thành công, ứng dụng đã khởi chạy");
                    mess = "Kích hoạt thành công, ứng dụng đã khởi chạy";
                }
                data.Active = entity.Active;
                
                await _repository.UpdateAsync(data);
                await _unitOfWork.SaveChangesAsync();
                return await Result<AutoSendTimer>.SuccessAsync(data, mess);
            }
            else
            {
                return await Result<AutoSendTimer>.FailAsync();
            }
        }
        public async Task StartJobEInvoiceAsync(AutoSendTimer entity)
        {
            var data = await _repository.Entities.SingleOrDefaultAsync(x => x.JobId == entity.JobId && x.ComId == entity.ComId);
            if (data != null)
            {
                var arrayPattern = JsonConvert.DeserializeObject<int[]>(data.PatternSerial);
                if (arrayPattern != null)
                {
                  
                    try
                    {
                        List<HistoryAutoSendTimer> historyAutoSendTimers = new List<HistoryAutoSendTimer>();
                        await _einvoice.SendCQTAutoAsync(historyAutoSendTimers, arrayPattern, entity.ComId, entity.TypeSupplierEInvoice);
                        historyAutoSendTimers.ForEach(x => x.IdAutoSendTimer = data.Id);
                        data.HistoryAutoSendTimers = historyAutoSendTimers;
                        await _repository.UpdateAsync(data);
                        await _unitOfWork.SaveChangesAsync();
                       
                    }
                    catch (Exception e)
                    {
                        await _unitOfWork.RollbackAsync();
                        _logger.LogInformation("Có lỗi trong quá trình xử lý dữ liệu gửi CQT");
                        _logger.LogInformation(e.Message);
                    }
                }
                else
                {
                    _logger.LogInformation("Không tìm thấy mẫu số ký hiệu");
                }
            }
            else
            {
                _logger.LogInformation("Không tìm thấy job để chạy"+ JsonConvert.SerializeObject(entity));
            }
        }

        public async Task<IResult> DeleteAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice)//xóa ứng dụng
        {
          var  getdata = await _repository.Entities.SingleOrDefaultAsync(x => x.Id == Id && x.ComId == Comid && x.TypeSupplierEInvoice == TypeSupplierEInvoice);
            if (getdata != null)
            {
                if (getdata.Active==true)
                {
                    RecurringJob.RemoveIfExists(getdata.JobId.ToString());
                }
                await _repository.DeleteAsync(getdata);
                await _unitOfWork.SaveChangesAsync();
                return await Result.SuccessAsync(HeperConstantss.SUS007);
            }
            return await Result.FailAsync(HeperConstantss.ERR012);
        } 
        public async Task<IResult> DeleteJobAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice)//xóa daily
        {
            var  getdata = await _repository.Entities.SingleOrDefaultAsync(x => x.Id == Id && x.ComId == Comid && x.TypeSupplierEInvoice == TypeSupplierEInvoice);
            if (getdata != null)
            {
                if (getdata.Active==true)
                {
                    RecurringJob.RemoveIfExists(getdata.JobId.ToString());
                }
                getdata.Active = false;
                await _repository.UpdateAsync(getdata);
                await _unitOfWork.SaveChangesAsync();
                return await Result.SuccessAsync("Đã tạm dừng ứng dụng");
            }
            return await Result.FailAsync("Không tìm thấy ứng dụng");
        } 
        public async Task<IResult> StartJobAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice)//chjay daily
        {
            var  getdata = await _repository.Entities.SingleOrDefaultAsync(x => x.Id == Id && x.ComId == Comid && x.TypeSupplierEInvoice == TypeSupplierEInvoice);
            if (getdata != null)
            {
                if (getdata.Active==false)
                {
                    RecurringJob.AddOrUpdate(getdata.JobId.ToString(), () => this.StartJobEInvoiceAsync(getdata), Cron.Daily(getdata.Hour, getdata.Minute), TimeZoneInfo.Local);
                    _logger.LogInformation("Kích hoạt thành công, ứng dụng đã khởi chạy");
                }
                getdata.Active = true;
                await _repository.UpdateAsync(getdata);
                await _unitOfWork.SaveChangesAsync();
                return await Result.SuccessAsync("Kích hoạt thành công, ứng dụng đã khởi chạy");
            }
            return await Result.FailAsync("Không tìm thấy ứng dụng");
        }
    }
}
