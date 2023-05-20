using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Query
{
   
    public class GetInvoiceArrayQuery : IRequest<Result<Invoice>>
    {
        public int[] LstIdInvoice { get; set; }
        public int ComId { get; set; }

        public class GetInvoiceArrayQueryHandler : IRequestHandler<GetInvoiceArrayQuery, Result<Invoice>>
        {
            private readonly IRepositoryAsync<Invoice> _repository;
            private readonly IInvoicePepository<Invoice> _Invoicerepository;
            public GetInvoiceArrayQueryHandler(IRepositoryAsync<Invoice> repository, IInvoicePepository<Invoice> Invoicerepository)
            {
                _Invoicerepository = Invoicerepository;
                _repository = repository;
            }
            public async Task<Result<Invoice>> Handle(GetInvoiceArrayQuery query, CancellationToken cancellationToken)
            {
                var Invoice = await _repository.Entities.Where(m => query.LstIdInvoice.Contains(m.Id) && m.ComId == query.ComId).Include(x => x.InvoiceItems).AsNoTracking().ToListAsync();
                
                var lstcodeinvoice = string.Join(",", Invoice.Select(x => x.InvoiceCode).ToArray());
                var getlstid = Invoice.Select(x => x.Id).ToArray();
                var result = query.LstIdInvoice.Except(getlstid).ToArray();// tức là lấy từ db ra kiểm tra xem đầy đủ các hóa đơn trên hệ thống không:Except yêu cầu đầu vào là hai danh sách. Nó trả về một danh sách mới với các phần tử từ danh sách đầu tiên không tồn tại trong danh sách thứ hai.
                if (result.Count()>0)
                {
                    var getExcept = _repository.Entities.Where(m => result.Contains(m.Id) && m.ComId == query.ComId).AsNoTracking();
                    var getCode = string.Join(",", getExcept.Select(x => x.InvoiceCode));
                    return await Result<Invoice>.FailAsync($"Các hóa đơn có mã: {getCode} đã bị xóa, vui lòng tải lại trang và thực hiện lại");
                }
                List<string> newlst = new List<string>();
                List<string> newlstismerge = new List<string>();
                foreach (var item in Invoice)
                {
                    if (item.IsMerge)
                    {
                        newlstismerge.Add(item.InvoiceCode);
                    }
                    else if (item.IdEInvoice!=null)
                    {
                        newlst.Add(item.InvoiceCode);
                    }
                }
                if (newlstismerge.Count()>0)
                {
                    var getCode = string.Join(",", newlstismerge.ToArray());
                    return await Result<Invoice>.FailAsync($"Các hóa đơn có mã: {getCode} đã phát hành hóa đơn điện tử gộp trước đó, không thể phát hành lại");
                } 
                if (newlst.Count()>0)
                {
                    var getCode = string.Join(",", newlst.ToArray());
                    return await Result<Invoice>.FailAsync($"Các hóa đơn có mã: {getCode} đã phát hành hóa đơn điện tử không thể phát hành lại");
                }
                var listiteminvoice = new List<InvoiceItem>();
                foreach (var item in Invoice)
                {
                    if (item.InvoiceItems.Count()==0)
                    {
                        return await Result<Invoice>.FailAsync($"Hóa đơn {item.InvoiceCode} không có dữ liệu sản phẩm không thể gộp!");
                    }
                    listiteminvoice.AddRange(item.InvoiceItems);
                }
                listiteminvoice.ForEach(x => x.Id = 0);
                float?[] vatRate = listiteminvoice.Where(x=>x.PriceNoVAT>0).Select(x => x.VATRate).Distinct().ToArray();

                var grinvocie = listiteminvoice.GroupBy(x=>x.Code);
                listiteminvoice = new List<InvoiceItem>();
                foreach (var item in grinvocie)
                {
                    var first = item.First();
                    var iteminvoice = new InvoiceItem()
                    {
                        Code = item.Key,
                        Name = item.First().Name,
                        Unit = item.First().Unit,
                        Price = item.First().Price,
                        PriceNoVAT = item.First().PriceNoVAT,
                        VATRate = item.First().VATRate,
                        TypeProductCategory = item.First().TypeProductCategory,
                        Quantity = item.Sum(x => x.Quantity),
                        Total = item.Sum(x => x.Total),
                        VATAmount = item.Sum(x => x.VATAmount),
                        Amonut = item.Sum(x => x.Amonut),
                    };
                    if (first.VATRate == (float)NOVAT.NOVAT && vatRate.Count()>0)
                    {
                        decimal vatrate = Convert.ToDecimal(vatRate[0]);
                        iteminvoice.PriceNoVAT = iteminvoice.Price;
                        iteminvoice.VATRate = vatRate[0];
                        iteminvoice.VATAmount = Math.Round(iteminvoice.Total * (vatrate / 100), MidpointRoundingCommon.Three);
                        iteminvoice.Amonut = Math.Round(iteminvoice.Total + iteminvoice.VATAmount, MidpointRoundingCommon.Three);

                    }
                    listiteminvoice.Add(iteminvoice);
                }

                var getfirstinvoice = Invoice.FirstOrDefault();
                getfirstinvoice.Total = Math.Round(listiteminvoice.Sum(x=>x.Total), MidpointRounding.AwayFromZero);
                getfirstinvoice.VATRate = vatRate.Count() > 0? vatRate[0]:10;
                getfirstinvoice.VATAmount = Math.Round(listiteminvoice.Sum(x=>x.VATAmount), MidpointRounding.AwayFromZero);
                getfirstinvoice.Amonut = Math.Round(listiteminvoice.Sum(x=>x.Amonut), MidpointRounding.AwayFromZero);
                getfirstinvoice.DiscountAmount = Invoice.Sum(x=>x.DiscountAmount);
                getfirstinvoice.InvoiceItems = listiteminvoice;
                return await Result<Invoice>.SuccessAsync(getfirstinvoice, lstcodeinvoice);
            }
        }
    }
}
