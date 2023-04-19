using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Infrastructure.Infrastructure.Repositories
{
    public class ReportPosRepository : IReportPosRepository
    {
        private readonly IRepositoryAsync<Customer> _repositoryCusomer;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;
        private readonly IRepositoryAsync<Product> _ProductRepository;
        private readonly IRepositoryAsync<PurchaseOrder> _PurchaseOrderRepository;
        public ReportPosRepository(IRepositoryAsync<Invoice> invoiceRepository,
            IRepositoryAsync<PurchaseOrder> PurchaseOrderRepository,
            IRepositoryAsync<Product> ProductRepository,
            IRepositoryAsync<Customer> repositoryCusomer,
            IOptions<CryptoEngine.Secrets> config)
        {
            _PurchaseOrderRepository = PurchaseOrderRepository;
            _ProductRepository = ProductRepository;
            _repositoryCusomer = repositoryCusomer;
            
            _config = config;
            _invoiceRepository = invoiceRepository;
        }
        public async Task<List<Invoice>> GetRevenue(SearchReportPosModel model)
        {
            var splitdate = model.rangesDate.Split("-");
            DateTime? startDate = Common.ConvertStringToDateTime(splitdate[0].Trim());
            DateTime? endDate = Common.ConvertStringToDateTime(splitdate[1].Trim()).Value.AddDays(1);
            var getdt = _invoiceRepository.Entities.Where(x => x.ComId == model.Comid && x.CreatedOn >= startDate && x.CreatedOn < endDate &&!x.IsDelete).Include(x=>x.RoomAndTable);
            return await getdt.ToListAsync();  
        }
        public async Task<List<Invoice>> GetByProduct(SearchReportPosModel model)
        {
            var splitdate = model.rangesDate.Split("-");
            DateTime? startDate = Common.ConvertStringToDateTime(splitdate[0].Trim());
            DateTime? endDate = Common.ConvertStringToDateTime(splitdate[1].Trim()).Value.AddDays(1);
            var getdt = _invoiceRepository.Entities.Where(x => x.ComId == model.Comid && x.CreatedOn >= startDate && x.CreatedOn < endDate && !x.IsDelete).Include(x=>x.InvoiceItems);
            return await getdt.ToListAsync();  
        }

        public async Task<List<Product>> GetOnhand(SearchReportPosModel model)
        {
            DateTime? startDate = DateTime.Now;
            if (!string.IsNullOrEmpty(model.FromDate))
            {
                startDate = Common.ConvertStringToDateTime(model.FromDate);
            }
            var getdt = _ProductRepository.Entities.Where(x => x.ComId == model.Comid);
            if (model.idCategory>0)
            {
                getdt = getdt.Where(x=>x.IdCategory== model.idCategory);
            } 
            if (!string.IsNullOrEmpty(model.productname))
            {
                getdt = getdt.Where(x=>x.Name.ToLower().Contains(model.productname.ToLower().Trim()));
            }  
            if (!string.IsNullOrEmpty(model.productcode))
            {
                getdt = getdt.Where(x=>x.Code.ToLower().Contains(model.productcode.ToLower().Trim()));
            }
            getdt = getdt.Include(x => x.CategoryProduct);
            return await getdt.ToListAsync();
        }

        public async Task<List<ReportXuatNhapTonKho>> GetExportImportOnhand(SearchReportPosModel model)
        {
            ResponseReport responseReport = new ResponseReport();
            var splitdate = model.rangesDate.Split("-");
            DateTime? startDate = Common.ConvertStringToDateTime(splitdate[0].Trim());
            DateTime? endDate = Common.ConvertStringToDateTime(splitdate[1].Trim()).Value.AddDays(1);


            var getPurchaseOrder = await _PurchaseOrderRepository.Entities.AsNoTracking().Where(x => x.Comid == model.Comid && x.CreatedOn >= startDate.Value && x.CreatedOn < endDate.Value).Include(x=>x.ItemPurchaseOrders).SelectMany(x=>x.ItemPurchaseOrders).ToListAsync();//lấy afhafng nhận trong kỳ
            var getdt =   _ProductRepository.Entities.AsNoTracking().Where(x => x.ComId == model.Comid);//lấy sản phẩm
           
            bool food = false;
            if (model.idCategory > 0)
            {
                food = true;
                getdt = getdt.Where(x => x.IdCategory == model.idCategory);
            }
            if (!string.IsNullOrEmpty(model.productname))
            {
                food = true;
                getdt = getdt.Where(x => x.Name.ToLower().Contains(model.productname.ToLower().Trim()));
            }
            if (!string.IsNullOrEmpty(model.productcode))
            {
                food = true;
                getdt = getdt.Where(x => x.Code.ToLower().Contains(model.productcode.ToLower().Trim()));
            }
            getdt = getdt.Include(x => x.CategoryProduct);
            var lstpro = await getdt.ToListAsync();
            //lấy invoice
            var getinvoicedetail = new List<InvoiceDetail>();
            var getinvoice = _invoiceRepository.Entities.AsNoTracking().Where(x => x.ComId == model.Comid&&(x.Status == EnumStatusInvoice.DA_THANH_TOAN || x.Status == EnumStatusInvoice.HOAN_TIEN_MOT_PHAN) && !x.IsDelete)
                .Include(x => x.InvoiceItems)
                .SelectMany(
                invoice => invoice.InvoiceItems,
                (invoice, InvoiceItem) => new InvoiceDetail
                {
                    IdProduct= (int)InvoiceItem.IdProduct,
                    IdInvoice= invoice.Id,
                    Name= InvoiceItem.Name,
                    CreatedOn = invoice.CreatedOn,
                    Price= InvoiceItem.Price,
                    Quantity= InvoiceItem.Quantity,
                    Unit= InvoiceItem.Unit,
                    Code= InvoiceItem.Code
                }
                );

            if (food)
            {
                List<int> arrpro = await lstpro.Select(x => x.Id).ToListAsync();
                getinvoice = getinvoice.Where(x=> arrpro.Contains(x.IdProduct));
            }
            var getAllData = await getinvoice.Where(x=>x.CreatedOn >= startDate).ToListAsync();//dungg cho đâu kỳ full
            var getdauky = await getAllData.GroupBy(x => x.Code).ToListAsync();//.Where(x => x.CreatedOn < endDate)
            var checktoncuoi = await getAllData.Where(x => x.CreatedOn >= endDate).GroupBy(x=>x.Code).ToListAsync();//tính cuối kỳ
            var checkdaxuattrongky = await getAllData.Where(x => x.CreatedOn < endDate).GroupBy(x=>x.Code).ToListAsync();//tính trong kỳ
            //tính toán lại
            List<ReportXuatNhapTonKho> ReportXuatNhapTonKho = new List<ReportXuatNhapTonKho>();
            foreach (var item in lstpro)
            {
                ReportXuatNhapTonKho reportXuatNhapTonKho = new ReportXuatNhapTonKho();
                reportXuatNhapTonKho.Id = item.Id;
                reportXuatNhapTonKho.Name = item.Name;
                reportXuatNhapTonKho.Code = item.Code;
                reportXuatNhapTonKho.Unit = item.Unit;
                reportXuatNhapTonKho.CategoryProductName = item.CategoryProduct?.Name;

                var getkey = getdauky.SingleOrDefault(x => x.Key == item.Code);
                reportXuatNhapTonKho.SoLuongTonDauKy = (getkey != null? getkey.Sum(x=>x.Quantity):0) + item.Quantity;
                reportXuatNhapTonKho.ThanhTienTonDauKy = reportXuatNhapTonKho.SoLuongTonDauKy * item.RetailPrice;

                var gettoncuoi = checktoncuoi.SingleOrDefault(x => x.Key == item.Code);
                reportXuatNhapTonKho.SoLuongTonCuoiKy = (gettoncuoi != null ? gettoncuoi.Sum(x => x.Quantity) : 0) + item.Quantity;
                reportXuatNhapTonKho.ThanhTienTonCuoiKy = reportXuatNhapTonKho.SoLuongTonCuoiKy * item.RetailPrice;

                var getdaxuattrongky = checkdaxuattrongky.SingleOrDefault(x => x.Key == item.Code);
                reportXuatNhapTonKho.SoLuongXuatTrongKy = (getdaxuattrongky != null ? getdaxuattrongky.Sum(x => x.Quantity) : 0);
                reportXuatNhapTonKho.ThanhTienXuatTrongKy = reportXuatNhapTonKho.SoLuongXuatTrongKy * item.RetailPrice;

                var getPurchaseOrders = getPurchaseOrder.GroupBy(x => x.Code).SingleOrDefault(x => x.Key == item.Code);
                reportXuatNhapTonKho.SoLuongNhapTrongKy = (getPurchaseOrders != null ? getPurchaseOrders.Sum(x => x.Quantity) : 0);
                reportXuatNhapTonKho.ThanhTienNhapTrongKy = reportXuatNhapTonKho.SoLuongNhapTrongKy * item.RetailPrice;
                ReportXuatNhapTonKho.Add(reportXuatNhapTonKho);

            }
            return ReportXuatNhapTonKho;
        }
    }
}
