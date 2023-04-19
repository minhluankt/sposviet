using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RevenueExpenditures.Query
{

    public class GetReportRevenueExpenditureQuery : IRequest<Result<ReportRevenueExpenditureModel>>
    {
        public EnumTypeRevenueExpenditure Type { get; set; }
        public string RangesDate { get; set; }
        public string Code { get; set; }
        public int ComId { get; set; }
        public GetReportRevenueExpenditureQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetReportCachedQueryHandler : IRequestHandler<GetReportRevenueExpenditureQuery, Result<ReportRevenueExpenditureModel>>
    {

        private readonly IRepositoryAsync<PaymentMethod> _repositoryPaymentMethod;
        private readonly IRepositoryAsync<RevenueExpenditure> _repository;
        private readonly IRepositoryAsync<Customer> _repositoryCustomer;
        private readonly IRepositoryAsync<Suppliers> _repositorySuppliers;
        private readonly IMapper _mapper;

        public GetReportCachedQueryHandler(
            IRepositoryAsync<RevenueExpenditure> repository, IRepositoryAsync<PaymentMethod> repositoryPaymentMethod,
            IRepositoryAsync<Customer> repositoryCustomer,
            IRepositoryAsync<Suppliers> repositorySuppliers,

            IMapper mapper)
        {
            _repositoryCustomer = repositoryCustomer;
            _repositorySuppliers = repositorySuppliers;
            _repositoryPaymentMethod = repositoryPaymentMethod;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<ReportRevenueExpenditureModel>> Handle(GetReportRevenueExpenditureQuery request, CancellationToken cancellationToken)
        {
            IQueryable<RevenueExpenditure> lst = _repository.GetAllQueryable().Where(x => x.ComId == request.ComId).AsNoTracking();
        
            if (!string.IsNullOrEmpty(request.RangesDate))
            {
                var _split = request.RangesDate.Split('-');
                DateTime? sratdate = Common.ConvertStringToDateTime(_split[0].Trim());
                DateTime? enddate = Common.ConvertStringToDateTime(_split[1].Trim());

               var gettondauky = lst.Where(m => m.CreatedOn < sratdate).GroupBy(x => x.Type).Select(x=>new ReportRevenueExpenditureDetailt()
               {
                   Amount = x.Sum(z=>z.Amount),
                   Type = x.Key
               }).ToList();
                decimal? tongthu = gettondauky.SingleOrDefault(x => x.Type == EnumTypeRevenueExpenditure.THU)?.Amount;
                decimal? tongchi = gettondauky.SingleOrDefault(x => x.Type == EnumTypeRevenueExpenditure.CHI)?.Amount;

                ReportRevenueExpenditureModel reportRevenueExpenditureModel = new ReportRevenueExpenditureModel();
                reportRevenueExpenditureModel.BeginningFund = (tongthu??0)-(tongchi??0);

               var getdaoanhthu = lst.Where(m => m.CreatedOn >= sratdate && m.CreatedOn < enddate.Value.AddDays(1)).GroupBy(x => x.Type).Select(x => new ReportRevenueExpenditureDetailt()
               {
                   Amount = x.Sum(z => z.Amount),
                   Type = x.Key
               }).ToList();//bao gồm thu và chi trong khoản thời gian khách chọn
               reportRevenueExpenditureModel.TotalRevenue= (getdaoanhthu.SingleOrDefault(x => x.Type == EnumTypeRevenueExpenditure.THU)?.Amount)??0;//thu
               reportRevenueExpenditureModel.TotalExpenditure = (getdaoanhthu.SingleOrDefault(x => x.Type == EnumTypeRevenueExpenditure.CHI)?.Amount)??0;//chi
                //tính tồn quỹ = tồn đầu kỳ + (thu - chi)
               reportRevenueExpenditureModel.FundsBalance = reportRevenueExpenditureModel.BeginningFund + (reportRevenueExpenditureModel.TotalRevenue- reportRevenueExpenditureModel.TotalExpenditure);
                return Result<ReportRevenueExpenditureModel>.Success(reportRevenueExpenditureModel);
            }
           
            return await Result<ReportRevenueExpenditureModel>.FailAsync("Không tìm thấy khoản thời gian cần thực hiện");
        }
    }
}
