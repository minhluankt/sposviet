using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;

namespace Application.Features.ReportPoss.Query
{

    public class GetReportsProductsQuery : SearchReportPosModel, IRequest<Result<ReportProduct>>
    {
        public class GetReportsProductsQueryHandler : IRequestHandler<GetReportsProductsQuery, Result<ReportProduct>>
        {
            private readonly IProductPepository<Product> _repositoryProduct;
            private readonly IReportPosRepository _repository;
            public GetReportsProductsQueryHandler(IReportPosRepository repository, IProductPepository<Product> repositoryProduct)
            {
                _repositoryProduct = repositoryProduct;
                _repository = repository;
            }
            public async Task<Result<ReportProduct>> Handle(GetReportsProductsQuery query, CancellationToken cancellationToken)
            {
                var getdt = await _repository.GetByProduct(query);
             
                ReportProduct ReportProducts = new ReportProduct();

                switch (query.typeReportProduct)
                {
                    case Enums.EnumTypeReportProduct.NONE:
                        break;
                    case Enums.EnumTypeReportProduct.DANHMUCMATHANG:
                        List<ItemReportProduct> ListItemReportProducts = new List<ItemReportProduct>();
                        // báo cáo theo sản phẩm
                        var InvoiceItems = getdt.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN).SelectMany(x => x.InvoiceItems).GroupBy(x => x.IdProduct).ToList();

                        foreach (var item in InvoiceItems)
                        {
                            //var Cate = _repositoryProduct.GetById(item.First().IdProduct.Value, true, false, false);

                            ListItemReportProducts.Add(new ItemReportProduct()
                            {
                                ProductName = item.First().Name,
                                Unit = !string.IsNullOrEmpty(item.First().Unit) ? item.First().Unit : string.Empty,
                                Price = item.First().Price,
                                EntryPrice = item.First().EntryPrice,
                                ProductCode = item.First().Code,
                                Quantity = item.Sum(x => x.Quantity),
                                Total = item.Sum(x => x.Total),
                                DiscountAmount = item.Sum(x => x.DiscountAmount),
                                CategoryName = item.First().IdProduct != null ? (_repositoryProduct.GetById(item.First().IdProduct.Value, true, false, false)?.CategoryProduct?.Name) : "Không xác định"
                            });
                        }
                        ReportProducts.ListItemReports = ListItemReportProducts;
                        // báo cáo chi tiết theo sản phẩm

                        var InvoiceItemsDetail = getdt.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN).SelectMany(invoice => invoice.InvoiceItems, (invoice, item) => new ItemReportProductDetailt()
                        {
                            ProductName = item.Name,
                            Unit = !string.IsNullOrEmpty(item.Unit) ? item.Unit : string.Empty,
                            Price = item.Price,
                            EntryPrice = item.EntryPrice,
                            ProductCode = item.Code,
                            Quantity = item.Quantity,
                            Total = item.Total,
                            DiscountAmount = item.DiscountAmount,
                            InvoiceNo = invoice.InvoiceCode,
                            IdInvoice = invoice.Id,
                            Buyer = invoice.Buyer,
                            CreateDate = invoice.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                            CusCode = !string.IsNullOrEmpty(invoice.CusCode) ? invoice.CusCode : string.Empty,

                        }).OrderByDescending(x => x.IdInvoice).ToList();

                        ReportProducts.ListItemReportDetails = InvoiceItemsDetail;
                        break;
                    case Enums.EnumTypeReportProduct.MATHANGBANCHAY:
                        List<ItemReportProduct> ListItemReportProductsBANCHAY = new List<ItemReportProduct>();
                        // báo cáo theo sản phẩm
                        var InvoiceItemsbanchay = getdt.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN).SelectMany(x => x.InvoiceItems).GroupBy(x => x.IdProduct).ToList();
                        // chart
                        var lstchart = new List<Chart>();
                        foreach (var item in InvoiceItemsbanchay)
                        {
                            //var Cate = _repositoryProduct.GetById(item.First().IdProduct.Value, true, false, false);

                            ListItemReportProductsBANCHAY.Add(new ItemReportProduct()
                            {
                                ProductName = item.First().Name,
                                Unit = !string.IsNullOrEmpty(item.First().Unit) ? item.First().Unit : string.Empty,
                                Price = item.First().Price,
                                EntryPrice = item.First().EntryPrice,
                                ProductCode = item.First().Code,
                                Quantity = item.Sum(x => x.Quantity),
                                Total = item.Sum(x => x.Total),
                                DiscountAmount = item.Sum(x => x.DiscountAmount),
                                CategoryName = item.First().IdProduct != null ? (_repositoryProduct.GetById(item.First().IdProduct.Value, true, false, false)?.CategoryProduct?.Name) : "Không xác định"
                            });
                            lstchart.Add(new Chart()
                            {
                                Key = item.First().Name,
                                Value = item.Sum(x => x.Total)
                            });
                        }
                        ReportProducts.Charts = lstchart;
                        ReportProducts.ListItemReports = ListItemReportProductsBANCHAY.OrderByDescending(x=>x.Quantity).ThenByDescending(x=>x.Total).ToList();

                       
                       
                        break;
                    default:
                        break;
                }
              
                return await Result<ReportProduct>.SuccessAsync(ReportProducts);
            }
        }
    }
}
