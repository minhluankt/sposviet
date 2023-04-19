using Domain.Entities;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IReportPosRepository
    {
        Task<List<Invoice>> GetRevenue(SearchReportPosModel model);
        Task<List<Product>> GetOnhand(SearchReportPosModel model);
        Task<List<ReportXuatNhapTonKho>> GetExportImportOnhand(SearchReportPosModel model);
        Task<List<Invoice>> GetByProduct(SearchReportPosModel model);
    }
}
