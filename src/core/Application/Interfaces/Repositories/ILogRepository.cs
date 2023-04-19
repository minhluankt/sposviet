using Application.DTOs.Logs;
using Application.Hepers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Application.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task DeleteAuditLogAsync();
        Task<List<AuditLogResponse>> GetAuditLogsAsync(int ComId, string userId);
        Task<IPagedList<AuditLogResponse>> GetAuditLogsPaginated(int ComId, string userId, string FromDate, string ToDate, string textSearch, int pageIndex, int pageSize);
        Task<IPagedList<AuditLogByUser>> GetAuditLogsDapperPaginated(int ComId, string userId, string FromDate, string ToDate, string textSearch, int pageIndex, int pageSize);

        Task AddLogAsync(int ComId, string action, string userId, string NewValues = "", string OldValues = "");
        // serilog
        IOrderedEnumerable<FileInfo> GetAllSerilog(string folder);
        bool DeleteSerilog(string fileName, string folder);
        string SerilogView(string fileName, string folder);
        void JobDeleteSerilog(string folder);
    }
}
