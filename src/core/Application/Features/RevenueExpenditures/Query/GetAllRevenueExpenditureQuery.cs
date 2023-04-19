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
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RevenueExpenditures.Query
{
    public class GetAllRevenueExpenditureQuery : DatatableModel, IRequest<Result<List<RevenueExpenditure>>>
    {
        public EnumTypeRevenueExpenditure Type { get; set; }
        public string RangesDate { get; set; }
        public string Code { get; set; }
        public int ComId { get; set; }
        public GetAllRevenueExpenditureQuery(int _comId, EnumTypeRevenueExpenditure type)
        {
            ComId = _comId;
            Type = type;
        }
    }

    public class GetAllRevenueExpenditureCachedQueryHandler : IRequestHandler<GetAllRevenueExpenditureQuery, Result<List<RevenueExpenditure>>>
    {

        private readonly IRepositoryAsync<PaymentMethod> _repositoryPaymentMethod;
        private readonly IRepositoryAsync<RevenueExpenditure> _repository;
        private readonly IRepositoryAsync<Customer> _repositoryCustomer;
        private readonly IRepositoryAsync<Suppliers> _repositorySuppliers;
        private readonly IMapper _mapper;

        public GetAllRevenueExpenditureCachedQueryHandler(
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

        public async Task<Result<List<RevenueExpenditure>>> Handle(GetAllRevenueExpenditureQuery request, CancellationToken cancellationToken)
        {
            IQueryable<RevenueExpenditure> lst = _repository.GetAllQueryable().Include(x => x.CategoryCevenue).Where(x => x.ComId == request.ComId&&x.Type == request.Type).AsNoTracking();
            if (!string.IsNullOrEmpty(request.Code))
            {
                lst = lst.Where(x => x.Code.ToLower()== request.Code.ToLower());
            }
            if (!string.IsNullOrEmpty(request.RangesDate))
            {
                var _split = request.RangesDate.Split('-');
                DateTime? sratdate = Common.ConvertStringToDateTime(_split[0].Trim());
                DateTime? enddate = Common.ConvertStringToDateTime(_split[1].Trim());
               
                lst = lst.Where(m => m.CreatedOn >= sratdate && m.CreatedOn < enddate.Value.AddDays(1));
            }
            int countitem = lst.Count();
              var kq =  lst.OrderByDescending(x=>x.Id).Skip(request.skip).Take(request.pageSize);
              var innerJoinRevenueExpenditure =
              (from revenueExpenditure in kq
              join PaymentMethod in _repositoryPaymentMethod.Entities on revenueExpenditure.IdPayment equals PaymentMethod.Id into gj
              from subpet in gj.DefaultIfEmpty()
              select new RevenueExpenditure() // result selector
              {
                  ObjectRevenueExpenditure = revenueExpenditure.ObjectRevenueExpenditure,
                  CreatedOn = revenueExpenditure.CreatedOn,
                  CreatedBy = revenueExpenditure.CreatedBy,
                  Content = revenueExpenditure.Content,
                  ComId = revenueExpenditure.ComId,
                  Id = revenueExpenditure.Id,
                  IdCustomer = revenueExpenditure.IdCustomer,
                  Date = revenueExpenditure.Date,
                  Code = revenueExpenditure.Code,
                  Title = revenueExpenditure.Title,
                  Typecategory = revenueExpenditure.Typecategory,
                  CustomerName = revenueExpenditure.CustomerName,
                  Amount = revenueExpenditure.Amount,
                  Status = revenueExpenditure.Status,
                  Type = revenueExpenditure.Type,
                  CodeOriginaldocument = revenueExpenditure.CodeOriginaldocument,
                  CategoryCevenueName = (revenueExpenditure.CategoryCevenue != null ? revenueExpenditure.CategoryCevenue.Name : string.Empty),
                  PaymentName = subpet.Name,
              }).ToList();

            //var innerJoinRevenueExpenditure = kq
            //  .Join(_repositoryPaymentMethod.GetAllEnumerable(),// inner sequence 
            //  RevenueExpenditure => RevenueExpenditure.IdPayment,    // outerKeySelector
            //  PaymentMethod => PaymentMethod.Id,  // innerKeySelector
            //  (RevenueExpenditure, PaymentMethod) => new RevenueExpenditure() // result selector
            //  {
            //      ObjectRevenueExpenditure = RevenueExpenditure.ObjectRevenueExpenditure,
            //      CreatedOn = RevenueExpenditure.CreatedOn,
            //      CreatedBy = RevenueExpenditure.CreatedBy,
            //      Content = RevenueExpenditure.Content,
            //      ComId = RevenueExpenditure.ComId,
            //      Id = RevenueExpenditure.Id,
            //      IdCustomer = RevenueExpenditure.IdCustomer,
            //      Date = RevenueExpenditure.Date,
            //      Code = RevenueExpenditure.Code,
            //      Title = RevenueExpenditure.Title,
            //      Typecategory = RevenueExpenditure.Typecategory,
            //      CustomerName = RevenueExpenditure.CustomerName,
            //      Amount = RevenueExpenditure.Amount,
            //      Status = RevenueExpenditure.Status,
            //      Type = RevenueExpenditure.Type,
            //      CodeOriginaldocument = RevenueExpenditure.CodeOriginaldocument,
            //      CategoryCevenueName = (RevenueExpenditure.CategoryCevenue != null ? RevenueExpenditure.CategoryCevenue.Name : string.Empty),
            //      PaymentName = PaymentMethod?.Name,
            //  }).ToList();

                foreach (var item in innerJoinRevenueExpenditure)
                {
                    switch (item.ObjectRevenueExpenditure)
                    {
                        case EnumTypeObjectRevenueExpenditure.KHACHHANG:
                        if (string.IsNullOrEmpty(item.CustomerName))
                        {
                            var _cutomer = await _repositoryCustomer.Entities.SingleOrDefaultAsync(x => x.Id == item.IdCustomer);
                            item.CustomerName = _cutomer?.Name;
                        }
                       
                            break;
                        case EnumTypeObjectRevenueExpenditure.DOITAC:
                        if (string.IsNullOrEmpty(item.CustomerName))
                        {
                            var _suppliers = await _repositorySuppliers.Entities.SingleOrDefaultAsync(x => x.Id == item.IdCustomer);
                            item.CustomerName = _suppliers?.Name;
                        }
                       
                        break;
                        case EnumTypeObjectRevenueExpenditure.DOITUONGKHAC:
                            break;
                        default:
                            break;
                    }
                }


            return Result<List<RevenueExpenditure>>.Success(innerJoinRevenueExpenditure, countitem.ToString());
        }
    }
}
