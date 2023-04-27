using Application.DTOs.Logs;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using AutoMapper;
using Dapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemVariable;
using X.PagedList;
using Yanga.Module.EntityFrameworkCore.AuditTrail.Models;

namespace Infrastructure.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly IDapperRepository _dapperdb;
        private readonly IMapper _mapper;
        private readonly ILogger<LogRepository> _logger;
        private readonly IRepositoryAsync<AuditLog> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;
        [Obsolete]
        private IHostingEnvironment Environment;

        [Obsolete]
        public LogRepository(IRepositoryAsync<AuditLog> repository,
            IUnitOfWork unitOfWork,
            IDapperRepository dapperdb, ILogger<LogRepository> logger, IHostingEnvironment _environment, IMapper mapper, IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _dapperdb = dapperdb;
            _repository = repository;
            _mapper = mapper;
            Environment = _environment;
            _logger = logger;
            _dateTimeService = dateTimeService;
        }

        public async Task AddLogAsync(int ComId, string action, string userId, string NewValues = "", string OldValues = "")
        {
            var audit = new AuditLog()
            {
                ComId = ComId,
                Type = action,
                OldValues = OldValues,
                NewValues = NewValues,
                UserId = userId,
                DateTime = _dateTimeService.Now
            };
            await _repository.AddAsync(audit);
        }

        public async Task<List<AuditLogResponse>> GetAuditLogsAsync(int ComId, string userId)
        {
            var logs = await _repository.Entities.Where(a =>a.ComId== ComId&& a.UserId == userId).OrderByDescending(a => a.Id).Take(250).ToListAsync();
            var mappedLogs = _mapper.Map<List<AuditLogResponse>>(logs);
            return mappedLogs;
        }
         public async Task DeleteAuditLogAsync()
        {
            DateTime dateTime = DateTime.Now.AddDays(-40);
            var logs = await _repository.Entities.Where(a => a.DateTime<= dateTime).ToListAsync();
            await  _repository.DeleteRangeAsync(logs);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// ////// serilog
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public void JobDeleteSerilog(string folder)
        {
            _logger.LogInformation("JobDeleteSerilog Check");
            DateTime dateTime = DateTime.Now.AddDays(-60);
            IOrderedEnumerable<FileInfo> fileInfos = this.GetAllSerilog(SystemVariableHelper.folderLog);
            var getFiledeletes = fileInfos.Where(m => m.CreationTime < dateTime);
            foreach (var item in getFiledeletes)
            {
                _logger.LogInformation("Xóa serilog định kỳ: file " + item.Name);
                try
                {
                    this.DeleteSerilog(item.Name, SystemVariableHelper.folderLog);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.ToString());
                }
            }
        }
        public bool DeleteSerilog(string fileName, string folder)
        {
            fileName = Path.Combine(this.Environment.ContentRootPath, folder + fileName);
            var fi1 = new FileInfo(fileName);
            if (fi1 != null)
            {
                fi1.Delete();
                return true;
            }
            return false;
        }
        public IOrderedEnumerable<FileInfo> GetAllSerilog(string folder)
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(this.Environment.ContentRootPath, folder));
            FileInfo[] files = di.GetFiles();
            IOrderedEnumerable<FileInfo> orderedFiles = files.OrderBy(f => f.CreationTime);
            return orderedFiles;
        }
        public string SerilogView(string fileName, string folder)
        {
            string Html = string.Empty;
            fileName = Path.Combine(this.Environment.ContentRootPath, folder + fileName);
            if (System.IO.File.Exists(fileName))
            {
                Stream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(stream);
                string logContent = streamReader.ReadToEnd().Replace(System.Environment.NewLine, "<br/>");
                Html = logContent;
                return Html;
            }
            Html = "Không tìm thấy dữ liệu";
            return Html;
        }

        public Task<IPagedList<AuditLogResponse>> GetAuditLogsPaginated(int ComId, string UserId, string FromDate, string ToDate, string textSearch, int pageIndex, int pageSize)
        {
            var data = _repository.GetAllQueryable().AsNoTracking().Where(x=>x.ComId== ComId);
            IList<AuditLogResponse> auditLogResponses;

            if (!string.IsNullOrEmpty(UserId))
            {
                data = data.Where(m => m.UserId == UserId);
            }
            if (!string.IsNullOrEmpty(textSearch))
            {
                data = data.Where(m => m.Type.Replace(" ", "").ToLower().Contains(textSearch.Replace(" ", "").ToLower()) ||
                m.NewValues != null ? m.NewValues.Replace(" ", "").ToLower().Contains(textSearch.Replace(" ", "").ToLower()) : m.NewValues.Contains(textSearch));
            }
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                DateTime _from = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime _to = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                data = data.Where(m => m.DateTime >= _from && m.DateTime < _to.AddDays(1));
            }
            auditLogResponses = _mapper.Map<List<AuditLogResponse>>(data.OrderByDescending(m => m.Id));
            return auditLogResponses.ToPagedListAsync(pageIndex, pageSize);
        }
        public Task<IPagedList<AuditLogByUser>> GetAuditLogsDapperPaginated(int ComId,string UserId, string FromDate, string ToDate, string textSearch, int pageIndex, int pageSize)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("UserId", UserId);
            param.Add("ComId", ComId);

            string sql = "SELECT [Users].FullName,[Users].LastName,[Users].FirstName,  [Users].UserName, [AuditLogs].*  FROM [AuditLogs]  LEFT OUTER JOIN Users ON [AuditLogs].UserId =[Users].Id";
            string where = string.Empty;
            int dem = 0;
            IEnumerable<AuditLogByUser> auditLogResponses;
            
            if (!string.IsNullOrEmpty(UserId))
            {
              
                where += "UserId = @UserId and";
                dem++;

            }  
            if (ComId>0)
            {
                where += "[AuditLogs].ComId = @ComId and";
                dem++;

            }
            if (!string.IsNullOrEmpty(textSearch))
            {
                param.Add("textSearch", textSearch.ToLower().Replace(" ", ""));

                where += "(LOWER(replace(NewValues,' ','')) LIKE '%'+@textSearch+'%' OR LOWER(replace(Type,' ','')) LIKE '%'+@textSearch+'%') and";
                dem++;
            }
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {

                DateTime _from = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime _to = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                param.Add("FromDate", _from.ToString("yyyy-MM-dd"));
                param.Add("ToDate", _to.AddDays(1).ToString("yyyy-MM-dd"));
                where += "DateTime BETWEEN @FromDate AND @ToDate and";
                dem++;
            }
            if (dem > 0)
            {
                sql += " where " + where;
                int checkand = sql.LastIndexOf("and");
                if (checkand > 0)
                {
                    sql = sql.Remove(checkand);
                    sql = sql.Replace("and", "and ");
                }

            }

            sql += " ORDER BY Id DESC";
            auditLogResponses = _dapperdb.GetAllIEnumerable<AuditLogByUser>(sql, param);
            return auditLogResponses.ToPagedListAsync(pageIndex, pageSize);
        }

     
    }

    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<AuditLogResponse, Audit>().ReverseMap();
        }
    }
}
