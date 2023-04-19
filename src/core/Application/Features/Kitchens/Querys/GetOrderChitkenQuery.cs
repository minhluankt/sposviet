using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Kitchens.Querys
{

    public class GetOrderChitkenQuery : IRequest<Result<KitChenModel>>
    {
        public int Comid { get; set; }
        public bool OrderByDateReady { get; set; }
        public EnumStatusKitchenOrder Status { get; set; } = EnumStatusKitchenOrder.MOI;

        public class GetOrderChitkenQueryHandler : IRequestHandler<GetOrderChitkenQuery, Result<KitChenModel>>
        {
            private readonly INotifyChitkenRepository _repository;

            public GetOrderChitkenQueryHandler(INotifyChitkenRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<KitChenModel>> Handle(GetOrderChitkenQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetAllNotifyOrder(query.Comid, query.Status).ToListAsync();

                if (product.Count() == 0)
                {
                    return await Result<KitChenModel>.SuccessAsync(new KitChenModel());
                }
                if (query.OrderByDateReady)
                {
                    product = product.OrderByDescending(x => x.DateReady).ToList();
                }
                else
                {
                    product = product.OrderBy(x => x.Id).ToList();
                }

                KitChenModel kitChenModel = new KitChenModel();
                // sự ưu tiên
                kitChenModel.OrderByPrioritiesModels = product.Select(x => new OrderByPrioritiesModel
                {
                    createDate = x.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                    updateDate = x.LastModifiedOn,
                    dateReady = x.DateReady,
                    orderCode = x.OrderCode,
                    orderStaff = x.Cashername,
                    proName = x.ProName,
                    quantity = x.Quantity,
                    tableName = x.RoomTableName,
                    idKitchen = x.IdKitchen,
                    detailtKitchenModels = x.DetailtKitchens.Select(z => new DetailtKitchenModel()
                    {
                        Cashername = z.Cashername,
                        IdKitchen = z.Id,
                        Note = z.Note,
                        IsRemove = z.IsRemove,
                        DateCancel = z.DateCancel,
                        Quantity = z.Quantity,
                        TypeKitchenOrder = z.TypeKitchenOrder,
                    }).ToList()
                }).ToList();


                /// thực đơn theo bàn
                kitChenModel.OrderByRoomModels = product.GroupBy(y => y.IdOrder).Select(x => new OrderByRoomModel
                {
                    tableName = x.FirstOrDefault()?.RoomTableName,
                    idRoomTable = x.FirstOrDefault()?.IdRoomTable,
                    idOrder = x.Key,
                    orderCode = x.FirstOrDefault()?.OrderCode,
                    quantity = x.Sum(x => x.Quantity),
                    OrderByRoomDetailtModels = x.Select(z => new OrderByRoomDetailtModel()
                    {
                        createDate = z.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                        orderStaff = z.Cashername,
                        updateDate = z.LastModifiedOn,
                        proName = z.ProName,
                        dateReady = z.DateReady,
                        quantity = z.Quantity,
                        idKitchen = z.IdKitchen,
                        tableName = z.RoomTableName,
                        detailtKitchenModels = z.DetailtKitchens.Select(c => new DetailtKitchenModel()
                        {
                            IsRemove = c.IsRemove,
                            Cashername = c.Cashername,
                            IdKitchen = c.Id,
                            Note = c.Note,
                            DateCancel = c.DateCancel,
                            Quantity = c.Quantity,
                            TypeKitchenOrder = c.TypeKitchenOrder,
                        }).ToList()
                    }).ToList()
                }).ToList();


                //thực đơn theo món
                kitChenModel.OrderByFoodModels = product.Where(x => x.Quantity > 0).GroupBy(y => y.IdProduct).Select(x => new OrderByFoodModel
                {
                    proName = x.FirstOrDefault()?.ProName,
                    proCode = x.FirstOrDefault()?.ProCode,
                    idProduct = x.FirstOrDefault()?.IdProduct,
                    quantity = x.Sum(x => x.Quantity),
                }).ToList();

                return await Result<KitChenModel>.SuccessAsync(kitChenModel);
            }
        }
    }
}
