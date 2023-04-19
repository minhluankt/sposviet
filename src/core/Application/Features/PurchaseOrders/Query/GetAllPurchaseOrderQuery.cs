using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PurchaseOrders.Query
{
    public class GetAllPurchaseOrderQuery : DatatableModel, IRequest<Result<MediatRResponseModel<List<PurchaseOrderModel>>>>
    {
        public string Code { get; set; }
        public int ComId { get; set; }
        public EnumTypePurchaseOrder Type { get; set; }
        public EnumStatusPurchaseOrder Status { get; set; } = EnumStatusPurchaseOrder.DA_NHAP_HANG;

        public GetAllPurchaseOrderQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetAllPurchaseOrderCachedQueryHandler : IRequestHandler<GetAllPurchaseOrderQuery, Result<MediatRResponseModel<List<PurchaseOrderModel>>>>
    {
        private readonly IPurchaseOrderRepository<PurchaseOrder> _repository;
        private readonly IMapper _mapper;
        public GetAllPurchaseOrderCachedQueryHandler(
            IPurchaseOrderRepository<PurchaseOrder> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MediatRResponseModel<List<PurchaseOrderModel>>>> Handle(GetAllPurchaseOrderQuery request, CancellationToken cancellationToken)
        {
            var lst = _repository.GetAll(request.Comid,request.Type, request.Code,request.skip,request.pageSize,request.sortColumn,request.sortColumnDirection);
            return await Result<MediatRResponseModel<List<PurchaseOrderModel>>>.SuccessAsync(lst);
        }
    }
}
