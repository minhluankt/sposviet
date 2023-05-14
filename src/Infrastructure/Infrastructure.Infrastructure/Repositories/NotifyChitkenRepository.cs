using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using HelperLibrary;
using Joker.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
namespace Infrastructure.Infrastructure.Repositories
{
    public class NotifyChitkenRepository : INotifyChitkenRepository
    {
        private IUnitOfWork _unitOfWork { get; }
        private readonly IDetailtKitchenRepository<DetailtKitchen> _detailtKitchenRepository;
        private readonly IRepositoryAsync<OrderTable> _ordertableRepository;
        private readonly IHistoryOrderRepository<HistoryOrder> _historyOrderRepository;
        private readonly IRepositoryAsync<OrderTableItem> _orderitemtableRepository;
        private readonly IRepositoryAsync<Kitchen> _kitchenRepository;
        private readonly ILogger<NotifyChitkenRepository> _log;
        public NotifyChitkenRepository(IRepositoryAsync<Kitchen> kitchenRepository,
            IRepositoryAsync<OrderTableItem> orderitemtableRepository,
            IDetailtKitchenRepository<DetailtKitchen> detailtKitchenRepository,
            ILogger<NotifyChitkenRepository> log,
            IUnitOfWork unitOfWork, IHistoryOrderRepository<HistoryOrder> historyOrderRepository,
            IRepositoryAsync<OrderTable> ordertableRepository)
        {
            _log = log;
            _detailtKitchenRepository = detailtKitchenRepository;
            _historyOrderRepository = historyOrderRepository;
            _orderitemtableRepository = orderitemtableRepository;
            _unitOfWork = unitOfWork;
            _kitchenRepository = kitchenRepository;
            _ordertableRepository = ordertableRepository;
        }

        public async Task<Result<List<NotifyOrderNewModel>>> NotifyOrder(int Comid, Guid Idorder,string Cashername, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC)
        {
            List<NotifyOrderNewModel> notify = new List<NotifyOrderNewModel>();
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var getorder = await _ordertableRepository.Entities.Include(x => x.RoomAndTable).Include(x => x.OrderTableItems).AsNoTracking().Where(x => x.ComId == Comid && x.IdGuid == Idorder && x.Status == EnumStatusOrderTable.DANG_DAT).SingleOrDefaultAsync();
                if (getorder == null)
                {
                    return Result<List<NotifyOrderNewModel>>.Fail(HeperConstantss.ERR012);
                }
                else
                {
                    // lưu các số lượng thông báo
                    var getitem = await _orderitemtableRepository.Entities.Where(x => x.IdOrderTable == getorder.Id).ToListAsync();
                    string randowm = LibraryCommon.RandomString(8);
                    var lst = new List<HistoryOrder>();
                    foreach (var item in getitem)
                    {
                        if (item.Quantity > item.QuantityNotifyKitchen)
                        {
                            //-----------adđ vào để in báo bếp
                            notify.Add(new NotifyOrderNewModel()
                            {
                                Code = item.Code,
                                Name = item.Name,
                                Unit = item.Unit,
                                Note = (item.QuantityNotifyKitchen == 0 ? item.Note : string.Empty),
                                RoomTableName = getorder.IsBringBack ? "Mang về" : getorder.RoomAndTable?.Name,
                                StaffName = Cashername,
                                Quantity = item.Quantity - item.QuantityNotifyKitchen,
                                Price = item.Price,//giá bán
                                TypeNotifyOrder = EnumTypeTemplatePrint.IN_BA0_CHE_BIEN
                            }); 
                            //-----------

                            var his = new HistoryOrder()
                            {
                                IdOrderTable = getorder.Id,
                                Carsher = Cashername,
                                Code = randowm,
                                Note = (item.QuantityNotifyKitchen == 0 ? item.Note : string.Empty),
                                IsNotif = true,
                                Quantity = item.Quantity - item.QuantityNotifyKitchen,
                                CreateDate = DateTime.Now,
                                Name = $"+ {(item.Quantity - item.QuantityNotifyKitchen).ToString("0.###")} {item.Name}",
                            };
                            lst.Add(his);
                        }

                    }
                    if (lst.Count() > 0)
                    {
                        await _historyOrderRepository.AddHistoryOrder(lst);//add lịch sử
                    }


                    getitem.ForEach(x => x.QuantityNotifyKitchen = x.Quantity);
                    await _orderitemtableRepository.UpdateRangeAsync(getitem);
                    await _unitOfWork.SaveChangesAsync();
                }
                getorder.CasherName = Cashername;
                var getorderchitken = await _kitchenRepository.Entities.Where(x => x.ComId == Comid && x.IdDichVu == IdDichVu && x.IdOrder == Idorder).AsNoTracking().ToListAsync();// phải check tất cả trạng thái 
                if (getorderchitken.Count() == 0)// chưa có thì thông báo mới
                {
                    await AddNotify(getorder.OrderTableItems.ToList(), getorder);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    return Result<List<NotifyOrderNewModel>>.Success(notify);
                }

                else
                {
                    bool update = false;
                    //var listremove = getorderchitken.Where(x => !getorder.OrderTableItems.Select(x => x.IdProduct).ToArray().Contains(x.Id));//tìm cái không có trong order đã thông báo bếp tức là đã bỏ đi k đặt nữa.
                   // foreach (var item in getorderchitken.GroupBy(x => x.IdProduct))
                    foreach (var item in getorderchitken.GroupBy(x => x.IdItemOrder))//đổi qua vì có thể có 1 sản phẩm hiển thị nhiều dòng, nên check item là đúng nhất
                    {
                        //var getitem = getorder.OrderTableItems.Where(x => x.IdProduct == item.Key).SingleOrDefault();
                        var getitem = getorder.OrderTableItems.Where(x => x.Id == item.Key).SingleOrDefault();
                        if (getitem != null)
                        {
                            decimal quantityold = item.Sum(x => x.Quantity);
                            if (getitem.Quantity > quantityold)
                            {
                                getitem.Quantity = getitem.Quantity - quantityold;
                                List<OrderTableItem> OrderTableItems = new List<OrderTableItem>();
                                if (getitem.QuantityNotifyKitchen>0)//tức là món mới chưa báo bếp lần nào mới cho add ghi chú, báo rồi thì set null
                                {
                                    getitem.Note = string.Empty;
                                }
                                OrderTableItems.Add(getitem);
                                await AddNotify(OrderTableItems, getorder);
                                update = true;
                            }
                        }
                    }

                    //var listnew = getorder.OrderTableItems.Where(x => !getorderchitken.Select(x => x.IdProduct).ToArray().Contains(x.IdProduct));//tìm cái không có trong order của khách tức là món mới cần đặt.
                    var listnew = getorder.OrderTableItems.Where(x => !getorderchitken.Select(x => x.IdItemOrder).ToArray().Contains(x.Id));//tìm cái không có trong order của khách tức là món mới cần đặt.
                    if (listnew.Count() > 0)
                    {
                        await AddNotify(listnew.ToList(), getorder);
                        update = true;
                    }
                    if (update)
                    {
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        return Result<List<NotifyOrderNewModel>>.Success(notify);
                    }
                    return Result<List<NotifyOrderNewModel>>.Fail(HeperConstantss.ERR046);
                }
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                return Result<List<NotifyOrderNewModel>>.Fail(e.Message);
            }
        }
        private async Task AddNotify(List<OrderTableItem> OrderTableItems, OrderTable getorder)
        {
            List<Kitchen> kitchens = new List<Kitchen>();
            foreach (var item in OrderTableItems)
            {
                Kitchen kitchen = new Kitchen();
                kitchen.IdDichVu = getorder.TypeProduct;
                kitchen.ComId = getorder.ComId;
                kitchen.IdOrder = getorder.IdGuid;
                kitchen.OrderCode = getorder.OrderTableCode;
                kitchen.IdRoomTable = getorder.IdRoomAndTableGuid;
                kitchen.IsBingBack = getorder.IsBringBack;
                kitchen.Buyer = getorder.Buyer;
                kitchen.Cashername = getorder.CasherName;
                kitchen.IdCashername = getorder.IdCasher;
                if (getorder.IsBringBack)
                {
                    kitchen.RoomTableName = "Mang về";
                }
                else
                {
                    kitchen.RoomTableName = getorder.RoomAndTable?.Name;
                }
                kitchen.ProName = item.Name;
                kitchen.ProCode = item.Code;
                kitchen.Note = item.Note;
                kitchen.IdProduct = item.IdProduct;
                kitchen.IdItemOrder = item.Id;
                kitchen.Quantity = item.Quantity;
                kitchen.Status = item.Status;
                if (item.Status == EnumStatusKitchenOrder.READY)
                {
                    kitchen.DateReady = DateTime.Now;
                }
                kitchens.Add(kitchen);
            }
            if (kitchens.Count() > 0)
            {
                await _kitchenRepository.AddRangeAsync(kitchens);

            }

        }

        public IQueryable<Kitchen> GetAllNotifyOrder(int Comid, EnumStatusKitchenOrder status = EnumStatusKitchenOrder.MOI)
        {
            return _kitchenRepository.GetAllQueryable().AsNoTracking().Include(x => x.DetailtKitchens).Where(x => x.ComId == Comid && x.Status == status);
        }

        public async Task<Result<int>> UpdateNotifyOrder(int Comid, Guid? IdOrder, Guid? IdKitchen, int? IdProduct, bool UpdateOne, EnumTypeNotifyKitchenOrder typeupdate, EnumStatusKitchenOrder Status, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC)
        {

            // kitchen nếu trong cùng 1 bàn cùng 1 order thì khi bấm done món nấu xong, thì qua phần chờ cung ứng nếu trùng món vs điều keienj đó thì tăng số lượng chứ k tạo thêm dòng.
            // nếu giảm = 0  rồi thì xóa luôn món ở pos
            switch (typeupdate)
            {
                case EnumTypeNotifyKitchenOrder.Orocessed://theo ưu tiên chuyển từ dg nấu sang đã nấu
                    await _unitOfWork.CreateTransactionAsync();
                    try
                    {
                        var queryOrocessed = _kitchenRepository.GetAllQueryable().Where(x => x.IdKitchen == IdKitchen && x.ComId == Comid);
                        if (Status == EnumStatusKitchenOrder.READY)
                        {
                            queryOrocessed = queryOrocessed.Where(x => x.Status == EnumStatusKitchenOrder.MOI);
                        }
                        else if (Status == EnumStatusKitchenOrder.DONE)
                        {
                            queryOrocessed = queryOrocessed.Where(x => x.Status == EnumStatusKitchenOrder.READY);
                        }
                        else
                        {
                            return Result<int>.Fail(HeperConstantss.ERR001);
                        }
                        var get = await queryOrocessed.Include(x => x.DetailtKitchens).SingleOrDefaultAsync();

                        if (get == null)
                        {
                            return Result<int>.Fail(HeperConstantss.ERR001);
                        }
                        if (UpdateOne)
                        {

                            if (get.Quantity <= 1)
                            {
                                if (Status == EnumStatusKitchenOrder.DONE)//tức là muốn done món thì update trạng thái thôi
                                {
                                    get.Status = Status;
                                    get.DateReady = DateTime.Now;
                                    get.DetailtKitchens.ForEach(x => x.IsRemove = true);
                                    await _kitchenRepository.UpdateAsync(get);
                                }
                                else
                                {
                                    var queryready = await _kitchenRepository.GetAllQueryable().OrderByDescending(x => x.DateReady).FirstOrDefaultAsync(x => x.IdItemOrder == get.IdItemOrder && x.IdOrder == get.IdOrder && x.ComId == Comid && x.Status == EnumStatusKitchenOrder.READY);
                                    if (queryready != null)// nếu có bên ready cùng bàn cùng order thì cộng thêm bên đó k sinh ra nhé
                                    {
                                        queryready.Quantity = queryready.Quantity + 1;
                                        await _kitchenRepository.UpdateAsync(queryready);//update qua bên kia


                                        await _detailtKitchenRepository.Remove(get.Id);//xóa cái chi tiết
                                        await _kitchenRepository.DeleteAsync(get);//xóa cái này đi
                                    }
                                    else
                                    {
                                        get.Status = Status;
                                        get.DateReady = DateTime.Now;
                                        get.DetailtKitchens.ForEach(x => x.IsRemove = true);
                                        await _kitchenRepository.UpdateAsync(get);
                                    }
                                }
                            }
                            else
                            {
                                if (Status == EnumStatusKitchenOrder.DONE)//tức là muốn done món thì update trạng thái thôi
                                {
                                    await this.CloneKitchen(get, Status);


                                }
                                else
                                {
                                    var queryready = await _kitchenRepository.GetAllQueryable().OrderByDescending(x => x.DateReady).FirstOrDefaultAsync(x => x.IdItemOrder == get.IdItemOrder && x.IdOrder == get.IdOrder && x.ComId == Comid && x.Status == EnumStatusKitchenOrder.READY);
                                    if (queryready != null)// nếu có bên ready cùng bàn cùng order thì cộng thêm bên đó k sinh ra nhé
                                    {
                                        queryready.Quantity = queryready.Quantity + 1;
                                        await _kitchenRepository.UpdateAsync(queryready);
                                    }
                                    else
                                    {
                                        await this.CloneKitchen(get, Status);

                                    }

                                }

                                get.Quantity = get.Quantity - 1;
                                if (get.Quantity == 0)
                                {
                                    return Result<int>.Fail(HeperConstantss.ERR001);
                                }
                                await _kitchenRepository.UpdateAsync(get);
                            }

                        }
                        else
                        {
                            if (Status == EnumStatusKitchenOrder.DONE)
                            {
                                get.DateReady = DateTime.Now;
                                get.Status = Status;
                                // xóa các his đi
                                get.DetailtKitchens.ForEach(x => x.IsRemove = true);
                                //await _detailtKitchenRepository.Remove(get.Id);
                                await _kitchenRepository.UpdateAsync(get);
                            }
                            else
                            {
                                var queryready = await _kitchenRepository.GetAllQueryable().OrderByDescending(x => x.DateReady).FirstOrDefaultAsync(x => x.IdItemOrder == get.IdItemOrder && x.IdOrder == get.IdOrder && x.ComId == Comid && x.Status == EnumStatusKitchenOrder.READY);
                                if (queryready != null)// nếu có bên ready cùng bàn cùng order thì cộng thêm bên đó k sinh ra nhé
                                {
                                    queryready.Quantity = queryready.Quantity + get.Quantity;
                                    await _kitchenRepository.UpdateAsync(queryready);//update qua bên kia

                                    await _detailtKitchenRepository.Remove(get.Id);//xóa cái chi tiết
                                    await _kitchenRepository.DeleteAsync(get);//xóa cái này đi
                                }
                                else
                                {
                                    get.DateReady = DateTime.Now;
                                    get.Status = Status;
                                    // xóa các his đi
                                    get.DetailtKitchens.ForEach(x => x.IsRemove = true);
                                    //await _detailtKitchenRepository.Remove(get.Id);
                                    await _kitchenRepository.UpdateAsync(get);
                                }
                            }


                        }
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();

                        return Result<int>.Success(HeperConstantss.SUS016);
                    }
                    catch (Exception e)
                    {
                        await _unitOfWork.RollbackAsync();
                        return Result<int>.Fail(e.Message);

                    }


                case EnumTypeNotifyKitchenOrder.UPDATEBYFOOD:
                    await _unitOfWork.CreateTransactionAsync();
                    try
                    {
                        var queryUPDATEBYFOOD = _kitchenRepository.GetAllQueryable().Where(x => x.IdProduct == IdProduct && x.ComId == Comid);
                        if (Status == EnumStatusKitchenOrder.READY)
                        {
                            queryUPDATEBYFOOD = queryUPDATEBYFOOD.Where(x => x.Status == EnumStatusKitchenOrder.MOI);
                        }
                        else if (Status == EnumStatusKitchenOrder.DONE)
                        {
                            queryUPDATEBYFOOD = queryUPDATEBYFOOD.Where(x => x.Status == EnumStatusKitchenOrder.READY);
                        }
                        var getlst = await queryUPDATEBYFOOD.Include(x => x.DetailtKitchens).OrderBy(x => x.Id).ToListAsync();
                        if (getlst.Count() == 0)
                        {
                            return Result<int>.Fail(HeperConstantss.ERR001);
                        }
                        if (UpdateOne)
                        {
                            var getone = getlst.FirstOrDefault();//lấy thèn đầu tiên là thèn mà bấm thông báo đầu tiên đã sắp xếp thứ tự
                            if (getlst.Count() == 1)
                            {
                                if (getone.Quantity < 1)
                                {
                                    return Result<int>.Fail(HeperConstantss.ERR001);
                                }
                                if (getone.Quantity == 1)
                                {
                                    if (Status == EnumStatusKitchenOrder.DONE)
                                    {
                                        getlst.ForEach(x => { x.Status = Status; x.DateReady = DateTime.Now; x.DetailtKitchens.ForEach(x => x.IsRemove = true); });
                                        await _kitchenRepository.UpdateRangeAsync(getlst);
                                    }
                                    else
                                    {
                                        var queryready = await _kitchenRepository.GetAllQueryable().SingleOrDefaultAsync(x => x.IdItemOrder == getone.IdItemOrder && x.IdOrder == getone.IdOrder && x.ComId == Comid && x.Status == EnumStatusKitchenOrder.READY);
                                        if (queryready != null)// nếu có bên ready cùng bàn cùng order thì cộng thêm bên đó k sinh ra nhé
                                        {
                                            queryready.Quantity = queryready.Quantity + 1;
                                            await _kitchenRepository.UpdateAsync(queryready);//update qua bên kia

                                            await _kitchenRepository.DeleteAsync(getone);//xóa cái này đi
                                        }
                                        else
                                        {
                                            getlst.ForEach(x => { x.Status = Status; x.DateReady = DateTime.Now; x.DetailtKitchens.ForEach(x => x.IsRemove = true); });
                                            await _kitchenRepository.UpdateRangeAsync(getlst);
                                        }

                                    }
                                }
                                else
                                {
                                    if (Status == EnumStatusKitchenOrder.DONE)
                                    {
                                        await this.CloneKitchen(getone, Status);
                                    }
                                    else
                                    {
                                        var queryready = await _kitchenRepository.GetAllQueryable().SingleOrDefaultAsync(x => x.IdItemOrder == getone.IdItemOrder && x.IdOrder == getone.IdOrder && x.ComId == Comid && x.Status == EnumStatusKitchenOrder.READY);
                                        if (queryready != null)// nếu có bên ready cùng bàn cùng order thì cộng thêm bên đó k sinh ra nhé
                                        {
                                            queryready.Quantity = queryready.Quantity + 1;
                                            await _kitchenRepository.UpdateAsync(queryready);
                                        }
                                        else
                                        {
                                            await this.CloneKitchen(getone, Status);
                                            // var kitchennew = new Kitchen();
                                            //kitchennew = getone.CloneJson();
                                            //kitchennew.Id = 0;
                                            //kitchennew.IdKitchen = Guid.NewGuid();
                                            //kitchennew.CreatedOn = DateTime.Now;
                                            //kitchennew.Quantity = 1;
                                            //kitchennew.Status = Status;
                                            //kitchennew.DateReady = DateTime.Now;
                                            //await _kitchenRepository.AddAsync(kitchennew);

                                        }
                                    }



                                    getlst.ForEach(x => x.Quantity = x.Quantity - 1);
                                    await _kitchenRepository.UpdateRangeAsync(getlst);
                                }
                            }
                            else
                            {
                                await this.CloneKitchen(getone, Status);

                                bool removeFirst = false;
                                if (getone.Quantity == 1)
                                {
                                    removeFirst = true;//tức là nếu thèn đầu tiên chỉ có 1 thì sau khi clone qua thì xóa đi luôn ở danh sách thông báo
                                }
                                getlst.ForEach(x =>
                                {
                                    if (x.Id == getone.Id && x.Quantity > 1)
                                    {
                                        x.Quantity = x.Quantity - 1;
                                    }
                                });
                                if (removeFirst)
                                {
                                    await _detailtKitchenRepository.Remove(getone.Id);
                                    await _kitchenRepository.DeleteAsync(getone);
                                }
                                else
                                {
                                    await _kitchenRepository.UpdateRangeAsync(getlst);
                                }

                            }

                        }
                        else
                        {
                            getlst.ForEach(x => { x.Status = Status; x.DateReady = DateTime.Now; x.DetailtKitchens.ForEach(x => x.IsRemove = true); });
                            await _kitchenRepository.UpdateRangeAsync(getlst);

                        }

                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        return Result<int>.Success(HeperConstantss.SUS016);
                    }
                    catch (Exception e)
                    {

                        await _unitOfWork.RollbackAsync();
                        return Result<int>.Fail(e.Message);
                    }

                case EnumTypeNotifyKitchenOrder.UPDATEBYTABLE: // theo đơn

                    var queryUPDATEBYTABLE = _kitchenRepository.GetAllQueryable().Include(x => x.DetailtKitchens).Where(x => x.IdOrder == IdOrder && x.ComId == Comid);
                    if (Status == EnumStatusKitchenOrder.READY)
                    {
                        queryUPDATEBYTABLE = queryUPDATEBYTABLE.Where(x => x.Status == EnumStatusKitchenOrder.MOI);
                    }
                    else if (Status == EnumStatusKitchenOrder.DONE)
                    {
                        queryUPDATEBYTABLE = queryUPDATEBYTABLE.Where(x => x.Status == EnumStatusKitchenOrder.READY);
                    }

                    var getlstroom = await queryUPDATEBYTABLE.ToListAsync();
                    if (getlstroom.Count() == 0)
                    {
                        return Result<int>.Fail(HeperConstantss.ERR001);
                    }
                    getlstroom.ForEach(x => { x.Status = Status; x.DateReady = DateTime.Now; x.DetailtKitchens.ForEach(x => x.IsRemove = true); });

                    await _kitchenRepository.UpdateRangeAsync(getlstroom);
                    await _unitOfWork.SaveChangesAsync();

                    return Result<int>.Success(HeperConstantss.SUS016);
                case EnumTypeNotifyKitchenOrder.DELETEKITCHEN:
                    await _unitOfWork.CreateTransactionAsync();
                    try
                    {
                        var qrDe = await _kitchenRepository.GetAllQueryable().SingleOrDefaultAsync(x => x.IdKitchen == IdKitchen && x.ComId == Comid);
                        if (qrDe == null)
                        {
                            return Result<int>.Fail(HeperConstantss.ERR001);
                        }
                        if (qrDe.Quantity > 0)
                        {
                            return Result<int>.Fail(HeperConstantss.ERR047);
                        }

                        await _detailtKitchenRepository.Remove(qrDe.Id);
                        await _kitchenRepository.DeleteAsync(qrDe);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        return Result<int>.Success(HeperConstantss.SUS016);
                    }
                    catch (Exception e)
                    {
                        return Result<int>.Fail(e.Message);
                    }


                default:
                    return Result<int>.Fail(HeperConstantss.ERR001);

            }


        }
        private async Task CloneKitchen(Kitchen entity, EnumStatusKitchenOrder Status)
        {
            var kitchennew = new Kitchen();
            kitchennew = entity.CloneJson();
            kitchennew.Id = 0;
            kitchennew.IdKitchen = Guid.NewGuid();
            kitchennew.CreatedOn = DateTime.Now;
            kitchennew.Quantity = 1;
            kitchennew.DateReady = DateTime.Now;
            kitchennew.Status = Status;
            await _kitchenRepository.AddAsync(kitchennew);
        }
       // public async Task UpdateNotifyKitchenCancelAsync(int Comid, Guid IdOrder, int IdProduct, decimal Quantity, string CasherName, string IdCasher, bool removeAllfood = false)//update các món hủy đi khi đã thông báo bếp
        public async Task UpdateNotifyKitchenCancelAsync(int Comid, Guid IdOrder, int IdItemOrder, decimal Quantity, string CasherName, string IdCasher, bool removeAllfood = false)//update các món hủy đi khi đã thông báo bếp
        {
            //checked điều kiện thêm nếu giả sử hủy nhiều số lượng của món đó, mà đã thông báo nhiều lần
            var getkitchen = await _kitchenRepository.Entities.Where(x => x.IdItemOrder == IdItemOrder && x.IdOrder == IdOrder && x.Quantity > 0 && x.ComId == Comid && (x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.READY || x.Status == EnumStatusKitchenOrder.DONE)).OrderBy(x => x.Status).ThenByDescending(x => x.Id).ToListAsync();
            if (getkitchen.Count() > 0)
            {
                if (getkitchen.FirstOrDefault().Quantity >= (Quantity * -1))// neeus  dòng đầu đã đủ thì thôi
                {
                    var getitem = getkitchen.FirstOrDefault();
                    getitem.Quantity = getitem.Quantity + (Quantity);
                    var his = new DetailtKitchen()
                    {
                        IdKitchen = getitem.Id,
                        IdCashername = IdCasher,
                        Cashername = CasherName,
                        Quantity = Quantity > 0 ? Quantity * -1 : Quantity,
                        DateCancel = DateTime.Now,
                        Note = $"{Quantity} {getitem.ProName}",
                    };
                    await _kitchenRepository.UpdateAsync(getitem);
                    _detailtKitchenRepository.Add(his);
                }
                else
                {
                    foreach (var item in getkitchen)
                    {
                        if (item.Quantity < (Quantity * -1))
                        {
                            var his = new DetailtKitchen()
                            {
                                IdKitchen = item.Id,
                                IdCashername = IdCasher,
                                Cashername = CasherName,
                                Quantity = item.Quantity * -1,
                                DateCancel = DateTime.Now,
                                Note = $"-{item.Quantity} {item.ProName}",
                            };
                            Quantity = item.Quantity + (Quantity);
                            item.Quantity = 0;
                            _detailtKitchenRepository.Add(his);

                        }
                        else
                        {
                            item.Quantity = item.Quantity + (Quantity);
                            var his = new DetailtKitchen()
                            {
                                IdKitchen = item.Id,
                                IdCashername = IdCasher,
                                Cashername = CasherName,
                                Quantity = Quantity > 0 ? Quantity * -1 : Quantity,
                                DateCancel = DateTime.Now,
                                Note = $"-{Quantity} {item.ProName}",
                            };
                            // await _kitchenRepository.UpdateAsync(getkitchen);
                            _detailtKitchenRepository.Add(his);
                            break;
                        }
                    }
                    await _kitchenRepository.UpdateRangeAsync(getkitchen);
                }
            }
            if (removeAllfood)// nếu cancel tất cả hủy món luôn
            {
               // var getcu = _kitchenRepository.Entities.Where(x => x.IdProduct == IdProduct && x.IdOrder == IdOrder && x.Quantity > 0 && x.ComId == Comid && (x.Status == EnumStatusKitchenOrder.DONE || x.Status == EnumStatusKitchenOrder.CANCEL));
                var getcu = _kitchenRepository.Entities.Where(x => x.IdItemOrder == IdItemOrder && x.IdOrder == IdOrder && x.Quantity > 0 && x.ComId == Comid && (x.Status == EnumStatusKitchenOrder.DONE || x.Status == EnumStatusKitchenOrder.CANCEL));
                if (getcu.Count() > 0)
                {
                    getcu.ForEach(x => x.IsCancelAll = true);
                    await _kitchenRepository.UpdateRangeAsync(getcu);
                }
            }
        }

        public async Task<List<NotifyOrderNewModel>> UpdateNotifyKitchenCancelListAsync(List<Kitchen> entity, int ComId,string CasherName,string IdCashername)
        {
            List<NotifyOrderNewModel> NotifyOrderNewModels = new List<NotifyOrderNewModel>();
            List<DetailtKitchen> lstde = new List<DetailtKitchen>();
            var getAll = _kitchenRepository.Entities.Where(x => x.ComId == ComId && entity.Select(x => x.IdOrder).ToArray().Contains(x.IdOrder) && x.Quantity > 0 && (x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.READY));

            foreach (var item in getAll)
            {
                ////
                var his = new DetailtKitchen()
                {
                    IdKitchen = item.Id,
                    IdCashername = IdCashername,
                    Cashername = CasherName,
                    Quantity = item.Quantity * -1,
                    DateCancel = DateTime.Now,
                    Note = $"-{item.Quantity} {item.ProName}",
                };
                //////
                /////-------------------add vào để báo bếp
                NotifyOrderNewModels.Add(new NotifyOrderNewModel()
                {
                    Note=item.Note,
                    Code=item.ProCode,
                    Name= item.ProName,
                    Quantity= item.Quantity,
                    RoomTableName= item.RoomTableName,
                    StaffName = item.Cashername,
                   
                });
                /////--------------------------
                lstde.Add(his);
                item.Quantity = 0;
            }

            await _kitchenRepository.UpdateRangeAsync(getAll);
            await _detailtKitchenRepository.AddRangeAsync(lstde);

            var getcu = _kitchenRepository.Entities.Where(x => entity.Select(x => x.IdOrder).ToArray().Contains(x.IdOrder) && x.Quantity > 0 && x.ComId == ComId && (x.Status == EnumStatusKitchenOrder.DONE || x.Status == EnumStatusKitchenOrder.CANCEL));
            if (getcu.Count() > 0)
            {
                getcu.ForEach(x => { x.IsCancelAll = true; });
                await _kitchenRepository.UpdateRangeAsync(getcu);
            }
            return NotifyOrderNewModels;
        }
        public async Task UpdateNotifyKitchenSpitOrderGraftAsync(int ComId, List<Guid> lstOrderOld, OrderTable ordernew, List<OrderTableItem> itemlistbyordernew, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC)
        {
            //ghép
            List<DetailtKitchen> lstde = new List<DetailtKitchen>();
            var getAll = _kitchenRepository.Entities.Where(x => x.ComId == ComId && lstOrderOld.Contains(x.IdOrder) && x.Quantity > 0 && (x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.READY)).OrderBy(x => x.Status).ThenByDescending(x => x.Id).ToList();
            List<HistoryOrder> lsthis = new List<HistoryOrder>();
            string randowm = LibraryCommon.RandomString(8);
            var today = DateTime.Now;
            foreach (var item in getAll)
            {
                var history = new HistoryOrder()
                {
                    IdOrderTable = ordernew.Id,
                    IdProduct = item.IdProduct.Value,
                    Carsher = ordernew.CasherName,
                    Code = randowm,
                    ProductName = item.ProName,
                    IsNotif = true,
                    Quantity = item.Quantity,
                    CreateDate = today,
                    NewTableName = ordernew.RoomAndTable != null ? ordernew.RoomAndTable.Name : "mang về",
                    OrderCode = ordernew.OrderTableCode,
                    TypeKitchenOrder = EnumTypeKitchenOrder.GHEP,
                    Name = $"+ {item.Quantity.ToString("0.###")} {item.ProName} (ghép từ {item.RoomTableName.ToLower()} sang)",
                };
                lsthis.Add(history);

                var his = new DetailtKitchen()
                {
                    IdKitchen = item.Id,
                    IdCashername = ordernew.IdCasher,
                    Cashername = ordernew.CasherName,
                    Quantity = item.Quantity,
                    DateCancel = DateTime.Now,
                    IsSpitOrder = true,
                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                    Note = $"Được chuyển từ {item.RoomTableName.ToLower()} sang  {(ordernew.RoomAndTable != null ? ordernew.RoomAndTable.Name.ToLower() : "mang về")} mã đơn {ordernew.OrderTableCode}",
                };  //////
                lstde.Add(his);
                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                var getitemnew = ordernew.OrderTableItems.Where(z => z.IdProduct == item.IdProduct).FirstOrDefault();
                item.IdItemOrder = getitemnew.Id;
                //----
                item.IdOrder = ordernew.IdGuid;
                item.RoomTableName = (ordernew.RoomAndTable != null ? ordernew.RoomAndTable.Name : "Mang về");
                item.IdRoomTable = ordernew.IdRoomAndTableGuid;
                item.OrderCode = ordernew.OrderTableCode;
            }
            await _kitchenRepository.UpdateRangeAsync(getAll);
            await _detailtKitchenRepository.AddRangeAsync(lstde);
           // List<OrderTableItem> orderTableItems = new List<OrderTableItem>();

            //var getListNewitemOrder = ordernew.OrderTableItems.Where(x => x.QuantityNotifyKitchen != x.Quantity).ToList();// tìm món mới ở bàn cần tách vào mà chưa thông báo bếp thì thông báo
            var _newitem = itemlistbyordernew.GroupBy(x => new { x.IdProduct }).Select(x => new OrderTableItem()
            {
                IdOrderTable = ordernew.Id,
                Quantity = x.Sum(z => z.Quantity),
                IdProduct = x.Key.IdProduct,
                Name = x.First()?.Name,
                Code = x.First()?.Code,
                Status = EnumStatusKitchenOrder.MOI,
                QuantityNotifyKitchen = x.Sum(z => z.Quantity)
            }).ToList();
            
            foreach (var item in _newitem)
            {
                var history = new HistoryOrder()
                {
                    IdOrderTable = ordernew.Id,
                    IdProduct = item.IdProduct.Value,
                    Carsher = ordernew.CasherName,
                    Code = randowm,
                    ProductName = item.Name,
                    IsNotif = true,
                    Quantity = item.Quantity,
                    CreateDate = today,
                    NewTableName = ordernew.RoomAndTable != null ? ordernew.RoomAndTable.Name : "mang về",
                    OrderCode = ordernew.OrderTableCode,
                    TypeKitchenOrder = EnumTypeKitchenOrder.THEM,
                    Name = $"+ {item.Quantity.ToString("0.###")} {item.Name}",
                };
                lsthis.Add(history);
            }
            await _historyOrderRepository.AddHistoryOrder(lsthis);
            await _detailtKitchenRepository.AddRangeAsync(lstde);
            await this.AddNotify(itemlistbyordernew, ordernew);
           
        }
        public async Task UpdateNotifyKitchenSpitOrderAsync(OrderTable OrderOld, List<OrderTableItem> lstorderold, int ComId, OrderTable newOrder, List<OrderTableItem> lstordernew = null, bool isCreatNewOrder = false,DateTime? _createDateHis = null)//hủy khi tách đơn
        {
            List<DetailtKitchen> lstde = new List<DetailtKitchen>();
            // lấy các sản phẩm còn lại trong đơn cũ là lstorderold
            //var getAll = await _kitchenRepository.Entities.Where(x => x.ComId == ComId && x.IdOrder == OrderOld.IdGuid && lstorderold.Select(z => z.IdProduct).Contains(x.IdProduct) && x.Quantity > 0 && (x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.READY || x.Status == EnumStatusKitchenOrder.DONE) && !x.IsCancelAll).OrderBy(x => x.Status).ThenByDescending(x => x.Id).ToListAsync();
            var getAll = await _kitchenRepository.Entities.Where(x => x.ComId == ComId && x.IdOrder == OrderOld.IdGuid && lstorderold.Select(z => z.Id).Contains(x.IdItemOrder) && x.Quantity > 0 && (x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.READY || x.Status == EnumStatusKitchenOrder.DONE) && !x.IsCancelAll).OrderBy(x => x.Status).ThenByDescending(x => x.IdItemOrder).ThenByDescending(x => x.Id).ToListAsync();
            bool isUpdategetAll = false;
            string randowm = LibraryCommon.RandomString(8);
            var lst = new List<HistoryOrder>();
            var lstoldOrder = new List<HistoryOrder>();// lịch sửa cho đơn cũ khi hủy món
            List<int> lstiditem = new List<int>();//list này để chứa các id sau khi update iditem vào trong kitchen, để lúc tìm iditem bên dưới thì trừ danh sách này ra
            DateTime CreateDateHis = _createDateHis ?? DateTime.Now;
            foreach (var item in lstorderold)
            {
                decimal quantityCheck = 0;

                decimal checkQuantityNotify = getAll.Where(x => x.IdItemOrder == item.Id).Sum(x => x.Quantity);
               // decimal checkQuantityNotify = getAll.Where(x => x.IdProduct == item.IdProduct).Sum(x => x.Quantity);
                if (checkQuantityNotify > item.QuantityNotifyKitchen)
                {
                  
                   
                    quantityCheck = checkQuantityNotify - item.QuantityNotifyKitchen;//số lượng cần chuyển đi

                    List<OrderTableItem> listOrderTableItem = new List<OrderTableItem>();// danh sách item sẽ thông báo cho đơn mới khi đã rút đi 1 ít số lượng từ đơn cũ, đơn cũ sẽ bị trừ đi A thì thông báo cho đơn mới A số lượng

                    foreach (var x in getAll)
                    {
                        //if (x.IdProduct == item.IdProduct)
                        if (x.IdItemOrder == item.Id)
                        {
                           // if (x.Status == EnumStatusKitchenOrder.READY || x.Status == EnumStatusKitchenOrder.MOI)
                            {
                                if (x.Quantity > quantityCheck)//lớn hơn thì trừ đi ít và thêm vào cho bàn mới đó
                                { 
                                    var history = new HistoryOrder()
                                    {
                                        IdOrderTable = item.IdOrderTable,
                                        IdProduct = item.IdProduct.Value,
                                        Carsher = newOrder.CasherName,
                                        Code = randowm,
                                        ProductName = item.Name,
                                        IsNotif = true,
                                        Quantity = quantityCheck * -1,
                                        CreateDate = CreateDateHis,
                                        NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                                        OrderCode = newOrder.OrderTableCode,
                                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                        Name = $"Đã chuyển {quantityCheck.ToString("0.###")} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                    };
                                    lst.Add(history);

                                    //quantityCheck = 0;
                                    var his = new DetailtKitchen()
                                    {
                                        IdKitchen = x.Id,
                                        IdCashername = newOrder.IdCasher,
                                        Cashername = newOrder.CasherName,
                                        Quantity = quantityCheck,
                                        DateCancel = CreateDateHis,
                                        IsSpitOrder = true,

                                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                        Note = $"Đã chuyển {quantityCheck.ToString("0.###")} {item.Name} sang  {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                    };
                                    lstde.Add(his);
                                    //////

                                    x.Quantity = x.Quantity - quantityCheck;
                                    isUpdategetAll = true;
                                    // sau khi trừ xong thì clone dòng  để thông báo cho đơn mới với số lượng đã chọn
                                   
                                    //----
                                    var newitem = item.CloneJson();// phari clone chusw k sẽ bị update lại luôn item này vào db
                                    newitem.Id = 0;
                                    newitem.IdItemOrderOld = x.IdItemOrder;//gán lại id cũ để xuống dưới update qua id mới phù hợp
                                    newitem.Quantity = quantityCheck;
                                    newitem.QuantityNotifyKitchen = quantityCheck;
                                    newitem.IdOrderTable = newOrder.Id;
                                    newitem.Status = x.Status;

                                    listOrderTableItem.Add(newitem);
                                    break;
                                }
                                else if (x.Quantity == quantityCheck)// nếu bằng thì update chính cái đó qua bàn mới và thoát for
                                {
                                    var history = new HistoryOrder()
                                    {
                                        ProductName = item.Name,
                                        IdProduct = item.IdProduct.Value,
                                        IdOrderTable = item.IdOrderTable,
                                        Carsher = newOrder.CasherName,
                                        Code = randowm,
                                        IsNotif = true,
                                        Quantity = quantityCheck * -1,
                                        CreateDate = CreateDateHis,
                                        NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                                        OrderCode = newOrder.OrderTableCode,
                                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                        Name = $"Đã chuyển {quantityCheck.ToString("0.###")} {item.Name} sang  {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                    };
                                    lst.Add(history);//

                                    var his = new DetailtKitchen()
                                    {
                                        IdKitchen = x.Id,
                                        IdCashername = newOrder.IdCasher,
                                        Cashername = newOrder.CasherName,
                                        Quantity = quantityCheck,
                                        DateCancel = CreateDateHis,
                                        IsSpitOrder = true,
                                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                        Note = $"Được chuyển từ {(!string.IsNullOrEmpty(x.RoomTableName) ? x.RoomTableName : "Mang về")} mã đơn {x.OrderCode} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                    };
                                    lstde.Add(his);
                                    //----tìm trên đơn mới đúng hàng hóa đó đúng sl để update luôn id item vào item clone mới,để tìm kiếm dc, và khi xóa món ở đơn cũng xóa luôn ở đây
                                    var getitemnew = lstordernew.Where(z => z.IdItemOrderOld == x.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                    if (getitemnew == null)
                                    {
                                        throw new Exception("Không tìm thấy item phù hợp");
                                    }
                                    x.IdItemOrder = getitemnew.Id;
                                    //----
                                    x.IdOrder = newOrder.IdGuid;
                                    x.IdRoomTable = newOrder.RoomAndTable?.IdGuid;
                                    x.RoomTableName = newOrder.RoomAndTable?.Name;
                                    x.Buyer = newOrder.Buyer;
                                    x.OrderCode = newOrder.OrderTableCode;
                                    x.IsBingBack = newOrder.IsBringBack;

                                    isUpdategetAll = true;
                                    break;
                                }
                                else if (x.Quantity < quantityCheck)//  thì được chạy nhánh bên dưới.
                                {
                                    quantityCheck = quantityCheck - x.Quantity;

                                    var history = new HistoryOrder()
                                    {
                                        ProductName = item.Name,
                                        IdProduct = item.IdProduct.Value,
                                        IdOrderTable = item.IdOrderTable,
                                        Carsher = newOrder.CasherName,
                                        Code = randowm,
                                        IsNotif = true,
                                        Quantity = quantityCheck * -1,
                                        CreateDate = CreateDateHis,
                                        NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                                        OrderCode = newOrder.OrderTableCode,
                                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                        Name = $"Đã chuyển {x.Quantity.ToString("0.###")} {item.Name} sang  {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                    };
                                    lst.Add(history);//

                                    var his = new DetailtKitchen()
                                    {
                                        IdKitchen = x.Id,
                                        IdCashername = newOrder.IdCasher,
                                        Cashername = newOrder.CasherName,

                                        Quantity = x.Quantity,
                                        DateCancel = CreateDateHis,
                                        IsSpitOrder = true,
                                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                        Note = $"Được chuyển từ {(!string.IsNullOrEmpty(x.RoomTableName) ? x.RoomTableName : "Mang về")} mã đơn {x.OrderCode} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                    };
                                    //////
                                    lstde.Add(his);
                                    //----tìm trên đơn mới đúng hàng hóa đó đúng sl để update luôn id item vào item clone mới,để tìm kiếm dc, và khi xóa món ở đơn cũng xóa luôn ở đây
                                    var getitemnew = lstordernew.Where(z => z.IdItemOrderOld == x.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                    if (getitemnew == null)
                                    {
                                        throw new Exception("Không tìm thấy item phù hợp");
                                    }
                                    x.IdItemOrder = getitemnew.Id;
                                    //-----
                                    x.IdOrder = newOrder.IdGuid;
                                    x.IdRoomTable = newOrder.RoomAndTable?.IdGuid;
                                    x.RoomTableName = newOrder.RoomAndTable?.Name;
                                    x.Buyer = newOrder.Buyer;
                                    x.OrderCode = newOrder.OrderTableCode;
                                    x.IsBingBack = newOrder.IsBringBack;
                                    // x.Quantity = 0;
                                    isUpdategetAll = true;
                                }
                            }
                          
                        }
                    }

                    if (listOrderTableItem.Count() > 0)//đây là danh sách báo mới cho đơn mới
                    {
                        //---------update lại iditemorder từ bảng orderitem sang báo bếp, vì list này là list new clone chứ k phải từ neworrder sang
                        listOrderTableItem.ForEach(x =>
                        {
                            //var getitem = lstordernew.Where(x=>x.IdProduct == x.IdProduct).OrderBy(x=>x.Id).FirstOrDefault();//OrderBy để lấy ông đầu tiên là ông cũ nhất vì ông mới thường sẽ có ghi chú, nên update ông cũ qua thôi
                            var getitem = lstordernew.Where(z=>z.IdItemOrderOld == x.IdItemOrderOld).OrderBy(x=>x.Id).FirstOrDefault();//OrderBy để lấy ông đầu tiên là ông cũ nhất vì ông mới thường sẽ có ghi chú, nên update ông cũ qua thôi
                            x.Id = getitem.Id;
                        });
                        await this.AddNotify(listOrderTableItem, newOrder); 
                    }

                }

            }
           
            if (lst.Count() > 0)
            {
                //check xem thông báo lịch sử cho đơn cũ đủ chưa
                var consolidatedChildren =
                  (from c in lst
                   group c by new
                   {
                       c.IdProduct
                   } into gcs
                   select new HistoryOrder()
                   {
                       CreateDate = CreateDateHis,
                       IsNotif = gcs.First().IsNotif,
                       TypeKitchenOrder = gcs.First().TypeKitchenOrder,
                       Quantity = gcs.Sum(c => c.Quantity),
                       IdOrderTable = gcs.First().IdOrderTable,
                       IdProduct = gcs.Key.IdProduct,
                       NewTableName = gcs.First()?.NewTableName,
                       OrderCode = gcs.First()?.OrderCode,
                       ProductName = gcs.First()?.ProductName,
                       Carsher = newOrder.CasherName,
                       Code = randowm,

                   }).ToList();
                lst = new List<HistoryOrder>();
                foreach (var itemhis in consolidatedChildren)// nhóm lại  và thông báo lại
                {
                    var getProductOldCheck = lstorderold.Where(x => x.IdProduct == itemhis.IdProduct).ToList();//đơn cũ còn
                    if (getProductOldCheck.Sum(x=>x.Quantity) > (itemhis.Quantity < 0 ? itemhis.Quantity * -1 : itemhis.Quantity))// 4
                    {
                        itemhis.Quantity = itemhis.Quantity;
                    }
                    itemhis.Name = $"Đã chuyển {(itemhis.Quantity < 0 ? itemhis.Quantity * -1 : itemhis.Quantity).ToString("0.###")} {itemhis.ProductName} sang {itemhis.NewTableName} mã {itemhis.OrderCode}";
                    lst.Add(itemhis);
                    // lịch sử cho đơn mới sau khi đã chuyển từ đơn cũ
                    var history = new HistoryOrder()
                    {
                        ProductName = itemhis.ProductName,
                        IdProduct = itemhis.IdProduct.Value,
                        IdOrderTable = newOrder.Id,
                        Carsher = newOrder.CasherName,
                        Code = randowm,
                        IsNotif = true,
                        Quantity = itemhis.Quantity * -1,
                        CreateDate = CreateDateHis,
                        NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                        OrderCode = newOrder.OrderTableCode,
                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                        Name = $"+ {(itemhis.Quantity < 0 ? itemhis.Quantity * -1 : itemhis.Quantity).ToString("0.###")} {itemhis.ProductName} (được chuyển từ {(OrderOld.RoomAndTable != null ? OrderOld.RoomAndTable.Name : "mang về").ToLower()} sang)",
                    };
                    lst.Add(history);
                }
               
                await _historyOrderRepository.AddHistoryOrder(lst);//add lịch sử
            }
           
            if (isUpdategetAll)
            {
                await _kitchenRepository.UpdateRangeAsync(getAll);
            }
            //danh sách này là danh sách không còn trong đơn cux luôn mà đã chuyển qua mói ưluoon.
            var getAllmove = await _kitchenRepository.Entities.Where(x => x.ComId == ComId && x.IdOrder == OrderOld.IdGuid && !lstorderold.Select(z => z.Id).Contains(x.IdItemOrder) && x.Quantity > 0 && (x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.READY)).OrderBy(x => x.Status).ThenByDescending(x => x.Id).ToListAsync();
            //var getAllmove = _kitchenRepository.Entities.Where(x => x.ComId == ComId && x.IdOrder == OrderOld.IdGuid && !lstorderold.Select(z => z.IdProduct).Contains(x.IdProduct) && x.Quantity > 0 && (x.Status == EnumStatusKitchenOrder.MOI || x.Status == EnumStatusKitchenOrder.READY)).OrderBy(x => x.Status).ThenByDescending(x => x.Id).ToList();
            if (getAllmove.Count() > 0)
            {
                lstiditem = new List<int>();
                decimal quantitycheckUpdate = 0;
               var lsthisnew = new List<HistoryOrder>();
                ////-------mục đích để nhóm lại đúng itemorder đúng với số lượng thực tế của item đó trong đơn, để dễ update cho item đơn mới
                //var listKitchenNew = new List<KitchenModel>();
                //var grbykitchen = getAllmove.GroupBy(x=>x.IdItemOrder);
                //foreach (var item in grbykitchen)
                //{
                //    listKitchenNew.Add(new KitchenModel() {
                //        IdItemOrder = item.Key,
                //        IdProduct = item.First()?.IdProduct,
                //        Quantity=item.Sum(x=>x.Quantity),
                //    });
                //}
                //foreach (var item in listKitchenNew)
                //{
                //    var getitemnew = lstordernew.Where(z => z.IdProduct == item.IdProduct && !lstiditem.Contains(z.Id) && z.Quantity == item.Quantity && z.Note == item.Note).OrderByDescending(x => x.Id).FirstOrDefault();
                //    if (getitemnew == null)
                //    {
                //        throw new Exception("Không tìm thấy item sp phù hợp");
                //    }
                //    lstiditem.Add(getitemnew.Id);
                //    item.IdItemOrderNew = getitemnew;
                //}
                ////-------------------

                foreach (var item in getAllmove)
                {
                    var his = new DetailtKitchen()
                    {
                        IdKitchen = item.Id,
                        IdCashername = newOrder.IdCasher,
                        Cashername = newOrder.CasherName,
                        Quantity = item.Quantity,
                        DateCancel = CreateDateHis,
                        IsSpitOrder = true,
                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                        Note = $"Chuyển từ {item.RoomTableName.ToLower()} mã đơn {item.OrderCode} sang  {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về").ToLower()} mã đơn {newOrder.OrderTableCode}",
                    };
                    lstde.Add(his);

                    //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                    //var getbyproduct
                    var getitemnew = lstordernew.Where(z => z.IdItemOrderOld == item.IdItemOrder).FirstOrDefault();
                    if (getitemnew ==null)
                    {
                        throw new Exception("Không tìm thấy item sp phù hợp"); 
                    }
                    item.IdItemOrder = getitemnew.Id;
                    //----
                    item.IdOrder = newOrder.IdGuid;
                    item.OrderCode = newOrder.OrderTableCode;
                    item.RoomTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về";
                    item.IdRoomTable = newOrder.IdRoomAndTableGuid;

                  
                    //lịch sử cho đơn mới  khi move  hết sp
                    var historymoi = new HistoryOrder()
                    {
                        ProductName = item.ProName,
                        IdProduct = item.IdProduct.Value,
                        IdOrderTable = newOrder.Id,
                        Carsher = newOrder.CasherName,
                        Code = randowm,
                        IsNotif = true,
                        Quantity = (item.Quantity<0? item.Quantity * -1: item.Quantity),
                        CreateDate = CreateDateHis,
                        NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                        OrderCode = newOrder.OrderTableCode,
                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                        Name = $"+ {(item.Quantity < 0 ? item.Quantity * -1 : item.Quantity).ToString("0.###")} {item.ProName} (được chuyển từ  {(OrderOld.RoomAndTable != null ? OrderOld.RoomAndTable.Name : "mang về").ToLower()} sang)",
                    };
                    lsthisnew.Add(historymoi); 
                    //lịch sử cho đơn cũ  khi move  hết sp
                    var historycu= new HistoryOrder()
                    {
                        ProductName = item.ProName,
                        IdProduct = item.IdProduct.Value,
                        IdOrderTable = OrderOld.Id,
                        Carsher = newOrder.CasherName,
                        Code = randowm,
                        IsNotif = true,
                        Quantity = (item.Quantity>0? item.Quantity * -1: item.Quantity),
                        CreateDate = CreateDateHis,
                        NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                        OrderCode = newOrder.OrderTableCode,
                        TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                        Name = $"Đã chuyển {(item.Quantity < 0 ? item.Quantity * -1 : item.Quantity).ToString("0.###")} {item.ProName} sang  {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về").ToLower()}",
                    };
                    lsthisnew.Add(historycu);

                }

               

                var consolidatedChildren =
                 (from c in lsthisnew
                  group c by new
                  {
                      c.IdProduct,c.IdOrderTable
                  } into gcs
                  select new HistoryOrder()
                  {
                      CreateDate = CreateDateHis,
                      IsNotif = gcs.First().IsNotif,
                      TypeKitchenOrder = gcs.First().TypeKitchenOrder,
                      Quantity = gcs.Sum(c => c.Quantity),
                      IdOrderTable = gcs.Key.IdOrderTable,
                      IdProduct = gcs.Key.IdProduct,
                      NewTableName = gcs.First()?.NewTableName,
                      OrderCode = gcs.First()?.OrderCode,
                      ProductName = gcs.First()?.ProductName,
                      Carsher = newOrder.CasherName,
                      Code = randowm,

                  }).ToList();
                lsthisnew = new List<HistoryOrder>();
                foreach (var itemhis in consolidatedChildren)// nhóm lại  và thông báo lại
                {
                    if (itemhis.IdOrderTable== newOrder.Id)
                    {
                        itemhis.Name = $"+ {itemhis.Quantity.ToString("0.###")} {itemhis.ProductName} (được chuyển từ  {(OrderOld.RoomAndTable != null ? OrderOld.RoomAndTable.Name : "mang về").ToLower()} sang)";
                        lsthisnew.Add(itemhis);
                    }
                    else
                    {
                        itemhis.Name = $"Đã chuyển {(itemhis.Quantity * -1).ToString("0.###")} {itemhis.ProductName} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về").ToLower()}";
                        lsthisnew.Add(itemhis);
                    }
                }

                await _historyOrderRepository.AddHistoryOrder(lsthisnew);//add lịch sử
                await _kitchenRepository.UpdateRangeAsync(getAllmove);
                await _unitOfWork.SaveChangesAsync();
            }

            //những món mới cần thông báo cho đơn mới, tức là món k liên quan trogn đơn cũ bỏ qua, mà là trường hợp bấm thêm đơn này xong chưa báo bếp, mà đã chuyển từ bàn kia sang
            if (lstordernew != null && isCreatNewOrder)// dành cho cả tách và ghép
            {
                var checkDonmoi = lstordernew.Where(x => x.Quantity != x.QuantityNotifyKitchen).ToList();//lstordernew là danh sách lọc ra các mo
                if (getAllmove.Count() > 0 && checkDonmoi.Count() > 0)// check nếu đơn cũ chuyển hết món A sang mới thì chỉ cần chuyển trực tiếp món đó sang luôn, tại getAllmove bên trên
                {
                    checkDonmoi = checkDonmoi.Where(x => !getAllmove.Select(z => z.IdProduct).Contains(x.IdProduct)).ToList();
                }
                if (checkDonmoi.Count() > 0)
                {
                    await this.AddNotify(checkDonmoi, newOrder);
                }
            }

            if (lstde.Count() > 0)
            {
                await _detailtKitchenRepository.AddRangeAsync(lstde);
            }


        }

        public async Task<Result<int>> UpdateNotifyAllStatusOrder(int Comid, Guid[] IdKitchen, EnumTypeNotifyKitchenOrder typeupdate, EnumStatusKitchenOrder Status = EnumStatusKitchenOrder.MOI, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var getAll = _kitchenRepository.Entities.Include(x => x.DetailtKitchens).Where(x => x.ComId == Comid && IdKitchen.Contains(x.IdKitchen) && x.Status == Status && x.IdDichVu == IdDichVu);

                var consolidatedChildren =
                from c in getAll
                group c by new
                {
                    //c.IdProduct,
                    c.IdItemOrder,
                    c.IdOrder
                } into gcs
                select new Kitchen()
                {
                    Quantity = gcs.Sum(c => c.Quantity),
                    IdOrder = gcs.Key.IdOrder,
                    IdItemOrder = gcs.Key.IdItemOrder,
                    IdProduct = gcs.First().IdProduct
                };
                List<Kitchen> lstrm = new List<Kitchen>();//list dể xóa đi
                if (Status == EnumStatusKitchenOrder.MOI)
                {
                    foreach (var item in consolidatedChildren)
                    {
                        //tìm neus có món đã chế beines xong của đúng đơn đó thì udpate số lượng vào luôn.
                       // var getDataItem = await _kitchenRepository.Entities.Where(x => x.IdProduct == item.IdProduct && x.IdOrder == item.IdOrder && x.Status == EnumStatusKitchenOrder.READY).OrderByDescending(x => x.DateReady).FirstOrDefaultAsync();
                        var getDataItem = await _kitchenRepository.Entities.Where(x => x.IdItemOrder == item.IdItemOrder && x.IdOrder == item.IdOrder && x.Status == EnumStatusKitchenOrder.READY).OrderByDescending(x => x.DateReady).FirstOrDefaultAsync();
                        if (getDataItem != null)//tìm dc tức là sản phẩm đó dg ready thì update vào nó còn bên mới thì xóa đi vì đa update 1.1
                        {
                            foreach (var itemkit in getAll)
                            {
                                //duyệt hết các sản phẩm đã update vì bên mới có thể có 2 dòng sp như nhau
                               // if (itemkit.IdProduct == getDataItem.IdProduct && itemkit.IdOrder == getDataItem.IdOrder)
                                if (itemkit.IdItemOrder == getDataItem.IdItemOrder && itemkit.IdOrder == getDataItem.IdOrder)
                                {
                                    lstrm.Add(itemkit);
                                }
                            }
                            getDataItem.Quantity = getDataItem.Quantity + item.Quantity; //1.1
                            await _kitchenRepository.UpdateAsync(getDataItem);
                        }
                    }
                }

                if (lstrm.Count() > 0 && Status == EnumStatusKitchenOrder.MOI)
                {
                    var lst = await getAll.Where(x => !lstrm.Select(z => z.IdKitchen).Contains(x.IdKitchen)).ToListAsync();// dnah sách này là lọc cái chưa update trên kia 1.1 để chuyển qua luôn
                    var lstremove = await getAll.Where(x => lstrm.Select(z => z.IdKitchen).Contains(x.IdKitchen)).ToListAsync();//danh sách remove
                    // lst.ForEach(x => { x.Status = EnumStatusKitchenOrder.READY; x.DateReady = DateTime.Now; });
                    lstrm = new List<Kitchen>();// tạo list rỗng để check
                    foreach (var item in lst)
                    {
                        var getitemcheck = lstrm.Where(x => x.IdItemOrder == item.IdItemOrder && x.IdOrder == item.IdOrder).FirstOrDefault();
                        if (getitemcheck==null)// phải check trong danh sách k có thì thực hiện xong add vào list lại để vòng sau checktieep
                        {
                            var getitem = await consolidatedChildren.Where(x => x.IdOrder == item.IdOrder && x.IdItemOrder == item.IdItemOrder).AsNoTracking().SingleOrDefaultAsync();
                            if (getitem != null)
                            {
                                item.Quantity = getitem.Quantity;
                                item.Status = EnumStatusKitchenOrder.READY;
                                item.DateReady = DateTime.Now;
                                item.DetailtKitchens.ForEach(z => z.IsRemove = true);//IsRemove là chẳng qua đẻ ẩn đi cái chi tiết thôi
                            }
                            lstrm.Add(item);
                        }
                        else
                        {
                            lstremove.Add(item);
                        }
                    }
                    lst = lst.Where(x => !lstremove.Select(z => z.Id).Contains(x.Id)).ToList();
                    await _kitchenRepository.UpdateRangeAsync(lst);
                    await _kitchenRepository.DeleteRangeAsync(lstremove);

                }
                else
                {
                    if (Status == EnumStatusKitchenOrder.MOI)
                    {
                        var listcheck = getAll.ToList();
                        foreach (var item in listcheck)
                        {
                            var getitemcheck = lstrm.Where(x => x.IdItemOrder == item.IdItemOrder && x.IdOrder == item.IdOrder).FirstOrDefault();
                            //if (!lstrm.Select(x => x.IdProduct).Contains(item.IdProduct))d
                            if (getitemcheck ==null) 
                            {
                                var getitem = await consolidatedChildren.Where(x => x.IdOrder == item.IdOrder && x.IdItemOrder == item.IdItemOrder).AsNoTracking().SingleOrDefaultAsync();
                                if (getitem != null)
                                {
                                    item.Quantity = getitem.Quantity;
                                    item.Status = EnumStatusKitchenOrder.READY;
                                    item.DateReady = DateTime.Now;
                                    item.DetailtKitchens.ForEach(z => z.IsRemove = true);
                                }
                                lstrm.Add(item);//là list cần giữ nhé
                            }
                        }
                        var lstrem = listcheck.Where(x => !lstrm.Select(z => z.Id).Contains(x.Id));
                        if (lstrem.Count()>0)
                        {
                            await _detailtKitchenRepository.RemoveRangeAsync(lstrem.Select(x => x.Id).ToList());
                            await _kitchenRepository.DeleteRangeAsync(lstrem);
                        }
                      

                        await _kitchenRepository.UpdateRangeAsync(lstrm);
                    }
                    else if (Status == EnumStatusKitchenOrder.READY)
                    {
                        getAll.ForEach(x =>
                        {
                            x.Status = EnumStatusKitchenOrder.DONE; x.DateDone = DateTime.Now;

                        }); await _kitchenRepository.UpdateRangeAsync(getAll.ToList());
                    }
                    else
                    {
                        return Result<int>.Fail(HeperConstantss.ERR001);
                    }


                    //var getdetai = _detailtKitchenRepository.UpdateRangeAsync(getAll.ToList());
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return Result<int>.Success(HeperConstantss.SUS006);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(e.Message);
            }

        }

        public async Task UpdateNotifyKitchenDoneListAsync(List<OrderTableItem> entity, OrderTable newOrder, Guid idorderOld, int ComId)
        {
            List<int> ints = new List<int>();
            List<OrderTableItem> listOrderTableItem = new List<OrderTableItem>();
            var getDone = await _kitchenRepository.Entities.Where(x => x.IdOrder == idorderOld && entity.Select(z => z.IdProduct).Contains(x.IdProduct) && x.Status == EnumStatusKitchenOrder.DONE).OrderByDescending(x => x.IdProduct).ToListAsync();
            decimal quantitycheck = 0;
            foreach (var item in getDone)
            {
                if (item.IdProduct.HasValue)
                {
                    var getid = entity.SingleOrDefault(x => x.IdProduct == item.IdProduct);
                    if (getid != null && !ints.Contains(item.IdProduct.Value))
                    {
                        quantitycheck = quantitycheck == 0 ? getid.Quantity : quantitycheck;
                        if (item.Quantity > quantitycheck)//nếu đã báo bếp 10 mà giwof chuyển đi 1 thì item.Quantitylà đã báo trước quantitycheck là số cần tách
                        {
                            item.Quantity = item.Quantity - quantitycheck;
                            var newitem = getid.CloneJson();// phari clone ra cái mới cho đơn kia là dã thông báo
                            newitem.IdOrderTable = newOrder.Id;
                            newitem.Status = EnumStatusKitchenOrder.DONE;
                            listOrderTableItem.Add(newitem);
                            quantitycheck = 0;
                            ints.Add(item.IdProduct.Value);

                        }
                        else if (item.Quantity == quantitycheck)
                        {
                            item.IdOrder = newOrder.IdGuid;
                            item.DateReady = DateTime.Now;
                            item.Cashername = newOrder.CasherName;
                            item.IdCashername = newOrder.IdCasher;
                            item.IsBingBack = newOrder.IsBringBack;
                            item.RoomTableName = newOrder.RoomAndTable?.Name;
                            quantitycheck = 0;
                            ints.Add(item.IdProduct.Value);

                        }
                        else if (item.Quantity < quantitycheck)
                        {
                            quantitycheck = quantitycheck - item.Quantity;

                            item.IdOrder = newOrder.IdGuid;
                            item.DateReady = DateTime.Now;
                            item.Cashername = newOrder.CasherName;
                            item.IdCashername = newOrder.IdCasher;
                            item.IsBingBack = newOrder.IsBringBack;
                            item.RoomTableName = newOrder.RoomAndTable?.Name;
                        }
                    }

                }

            }
            await _kitchenRepository.UpdateRangeAsync(getDone);
            if (listOrderTableItem.Count() > 0)
            {
                await this.AddNotify(listOrderTableItem, newOrder);

            }

        }

        public async Task NotifyOrderByItem(List<OrderTableItem> entity, OrderTable order, string CasherName,string IdCasher)
        {
            order.CasherName = CasherName;
            order.IdCasher = IdCasher;
            await this.AddNotify(entity,order);
        }
        public async Task UpdateNotifyKitchenTachdonVaoDonDacoAsync(OrderTable OrderOld, List<OrderTableItem> lstorderold, int ComId,
            OrderTable newOrder, List<OrderTableItem> lstordernew, List<OrderTableItem> lstorderoldremove, string CasherName, string IdCasher)
        {
           
            var getDonCus = _kitchenRepository.Entities.Where(x => x.ComId == ComId && x.IdOrder == OrderOld.IdGuid && !x.IsCancelAll).ToList();
            var lsthis = new List<HistoryOrder>();
            var lstdetail = new List<DetailtKitchen>();
            var lstOrderTableItemDonCu = new List<OrderTableItem>();
            var lstOrderTableItemDonMoi = new List<OrderTableItem>();

            string randowm = LibraryCommon.RandomString(8);
            DateTime today = DateTime.Now;
            foreach (var item in lstorderold)//duyệt từng cái còn lại của đơn cũ
            {
                var getitemtukitchen = getDonCus.Where(x => x.IdItemOrder == item.Id).ToList();
                //var getitemtukitchen = getDonCus.Where(x => x.IdProduct == item.IdProduct).ToList();
                var getitemsl = getitemtukitchen.Sum(x=>x.Quantity);//lấy sl đã báo
                var getprnew = lstordernew.SingleOrDefault(x => x.IdGuid == item.IdGuid);//lấy số lượng thực cần tách từ danh sách đã chọn ở view

                decimal quantitygoc = item.Quantity + (getprnew == null?0: getprnew.Quantity);// cộng lại để lấy đúng cái gốc ban đầu rồi đi check
                if (getprnew == null)//tìm k có tức là món k cần tách vẫn ở đơn cũ, check xem luôn cần báo số lượng món chưa báo bếp cho đơn cũ
                {
                    //chỉ có th lớn hơn thôi tức là sp còn chưa  báo bếp
                    if (quantitygoc > getitemsl)
                    {
                        var _st = item.CloneJson();
                        _st.Quantity = quantitygoc - getitemsl;
                        _st.IdOrderTable = OrderOld.Id;
                        lstOrderTableItemDonCu.Add(_st);
                        //await this.AddNotify(listCHikencanbao, OrderOld);
                        // thông báo cho đơn cũ vs sl mới luôn
                        var history = new HistoryOrder()
                        {
                            IdOrderTable = OrderOld.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = getitemsl,
                            CreateDate = today,
                            NewTableName = OrderOld.RoomAndTable != null ? OrderOld.RoomAndTable.Name : "mang về",
                            OrderCode = OrderOld.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.THEM,
                            Name = $"+ {_st.Quantity.ToString("0.###")} {item.Name}",
                        };
                        lsthis.Add(history);

                    }
                }
                else
                {
                    // trường hợp là có chọn sản phẩm từ đơn cũ cần ghép vào đươn mới
                    var quantitycheck = quantitygoc - getitemsl;//sl tổng - đi cái đã tb
                    if (quantitycheck == 0)//TH là đơn gốc đã thông báo đuinsg sl, cần chuyển thẳng qua đơn mới
                    {
                        var _historycu = new HistoryOrder()//lịch sử đơn cũ
                        {
                            IdOrderTable = item.IdOrderTable,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = getprnew.Quantity * -1,
                            CreateDate = DateTime.Now,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Name = $"Đã chuyển {getprnew.Quantity.ToString("0.###")} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                        };
                        lsthis.Add(_historycu);

                        var _historymoi = new HistoryOrder()//lịch sử đơn mới
                        {
                            IdOrderTable = newOrder.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = getprnew.Quantity * -1,
                            CreateDate = DateTime.Now,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Name = $"+ {getprnew.Quantity} {item.Name} (được chuyển từ {(OrderOld.RoomAndTable != null ? OrderOld.RoomAndTable.Name : "mang về")} mã đơn {OrderOld.OrderTableCode})"
                        };
                        lsthis.Add(_historymoi);

                        // int slcantim =
                        bool updatekitchen = false;
                        foreach (var kitchen in getitemtukitchen.OrderBy(x => x.Status).ThenBy(x => x.Quantity))//lọc qua các item đã báo giảm đi đủ sl giảm, lọc từ mới và slnhor
                        {
                            decimal soluongcheck = getprnew.Quantity;
                            if (kitchen.Quantity > soluongcheck)
                            {
                                //quantityCheck = 0;
                                var his = new DetailtKitchen()
                                {
                                    IdKitchen = kitchen.Id,
                                    IdCashername = IdCasher,
                                    Cashername = CasherName,
                                    Quantity = soluongcheck*-1,
                                    DateCancel = DateTime.Now,
                                    IsSpitOrder = true,

                                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                    Note = $"Đã chuyển {soluongcheck} {item.Name} sang  {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                };
                                lstdetail.Add(his);
                                //////
                                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                                var getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == kitchen.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                if (getitemnew == null)
                                {
                                    throw new Exception("Không tìm thấy item phù hợp");
                                }

                                var newitem = item.CloneJson();// phari clone chusw k sẽ bị update lại luôn item này vào db
                                newitem.Id = getitemnew.Id;
                                newitem.Quantity = soluongcheck;
                                newitem.QuantityNotifyKitchen = soluongcheck;
                                newitem.IdOrderTable = newOrder.Id;
                                newitem.Status = kitchen.Status;
                                lstOrderTableItemDonMoi.Add(newitem);

                                kitchen.Quantity = kitchen.Quantity - soluongcheck;
                                updatekitchen = true;

                                // sau khi trừ xong thì phải thông báo cho đơn mới.

                                break; //thoát vì đủ số lượng rồi

                            }
                            else if (kitchen.Quantity == soluongcheck)//= là move thẳng qua cho đơn mới
                            {
                               
                                //quantityCheck = 0;
                                var his = new DetailtKitchen()
                                {
                                    IdKitchen = kitchen.Id,
                                    IdCashername = IdCasher,
                                    Cashername = CasherName,
                                    Quantity = soluongcheck,
                                    DateCancel = DateTime.Now,
                                    IsSpitOrder = true,

                                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                    Note = $"Được chuyển từ {(!string.IsNullOrEmpty(kitchen.RoomTableName) ? kitchen.RoomTableName : "Mang về")} mã đơn {kitchen.OrderCode} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                };
                                lstdetail.Add(his);
                                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                                var getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == kitchen.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                if (getitemnew == null)
                                {
                                    throw new Exception("Không tìm thấy item phù hợp");
                                }

                                kitchen.IdItemOrder = getitemnew.Id;
                                //----
                                kitchen.IdOrder = newOrder.IdGuid;
                                kitchen.IdRoomTable = newOrder.RoomAndTable?.IdGuid;
                                kitchen.RoomTableName = newOrder.RoomAndTable?.Name;
                                kitchen.Buyer = newOrder.Buyer;
                                kitchen.OrderCode = newOrder.OrderTableCode;
                                kitchen.IsBingBack = newOrder.IsBringBack;
                                updatekitchen = true;
                                break;
                            }
                            else if (kitchen.Quantity < soluongcheck)
                            {
                                soluongcheck = soluongcheck - kitchen.Quantity;
                                var his = new DetailtKitchen()
                                {
                                    IdKitchen = kitchen.Id,
                                    IdCashername = IdCasher,
                                    Cashername = CasherName,
                                    Quantity = soluongcheck,
                                    DateCancel = DateTime.Now,
                                    IsSpitOrder = true,

                                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                    Note = $"Được chuyển từ {(!string.IsNullOrEmpty(kitchen.RoomTableName) ? kitchen.RoomTableName : "Mang về")} mã đơn {kitchen.OrderCode} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                };
                                lstdetail.Add(his);
                                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                                var getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == kitchen.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                if (getitemnew == null)
                                {
                                    throw new Exception("Không tìm thấy item phù hợp");
                                }

                                kitchen.IdItemOrder = getitemnew.Id;
                                //----

                                kitchen.IdOrder = newOrder.IdGuid;
                                kitchen.IdRoomTable = newOrder.RoomAndTable?.IdGuid;
                                kitchen.RoomTableName = newOrder.RoomAndTable?.Name;
                                kitchen.Buyer = newOrder.Buyer;
                                kitchen.OrderCode = newOrder.OrderTableCode;
                                kitchen.IsBingBack = newOrder.IsBringBack;
                                updatekitchen = true;
                            }
                        }
                        if (updatekitchen)
                        {
                            await _kitchenRepository.UpdateRangeAsync(getitemtukitchen);
                        }

                    }
                    else if (quantitycheck> 0 && quantitycheck< getprnew.Quantity)//trường hợp sl đơn cũ còn chưa thông báo còn dư mà thiếu so vs sl tách
                    {
                        decimal _slthongbaomoidonmoi = quantitycheck;
                        decimal _slthongbaochuyendoncuquadonmoi = getprnew.Quantity - quantitycheck;
                        //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                        var getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == item.Id).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                        if (getitemnew == null)//mục đích là tìm cái id mới dựa vào id cũ đã chọn từ đơn cũ
                        {
                            throw new Exception("Không tìm thấy item phù hợp");
                        }
                        var listCHikencanbao = new List<OrderTableItem>();
                        var _st = item.CloneJson();
                        _st.Quantity = _slthongbaomoidonmoi;
                        _st.IdOrderTable = newOrder.Id;
                        _st.Id = getitemnew.Id;
                        lstOrderTableItemDonMoi.Add(_st);
                        // thông báo cho đơn mới vs sl mới luôn
                        var history = new HistoryOrder()
                        {
                            IdOrderTable = newOrder.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = _st.Quantity,
                            CreateDate = today,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.THEM,
                            Name = $"+ {_st.Quantity.ToString("0.###")} {item.Name}",
                            // Name = $"Đã chuyển {quantityCheck} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                        };
                        lsthis.Add(history);

                        // thông báo cho đơn mới vs sl được chuyển qua
                        var historycumoi = new HistoryOrder()
                        {
                            IdOrderTable = newOrder.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = _slthongbaochuyendoncuquadonmoi,
                            CreateDate = today,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Name = $"+ {_slthongbaochuyendoncuquadonmoi.ToString("0.###")} {item.Name} (được chuyển từ {(OrderOld.RoomAndTable != null ? OrderOld.RoomAndTable.Name : "mang về")} sang)",
                            // Name = $"Đã chuyển {quantityCheck} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                        };
                        lsthis.Add(historycumoi);

                        //thông báo cho đơn cũ đã chuyển sl qua đơn mới
                        var historycu = new HistoryOrder()//lịch sử đơn cũ
                        {
                            IdOrderTable = item.IdOrderTable,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = _slthongbaochuyendoncuquadonmoi * -1,
                            CreateDate = DateTime.Now,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Name = $"Đã chuyển {_slthongbaochuyendoncuquadonmoi} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                        };
                        lsthis.Add(historycu);

                        // int slcantim =
                        bool updatekitchen = false;
                        foreach (var kitchen in getitemtukitchen.OrderBy(x=>x.Status).ThenBy(x=>x.Quantity))//lọc qua các item đã báo giảm đi đủ sl giảm, lọc từ mới và slnhor
                        {
                            if (kitchen.Quantity> _slthongbaochuyendoncuquadonmoi)
                            {
                                //quantityCheck = 0;
                                var his = new DetailtKitchen()
                                {
                                    IdKitchen = kitchen.Id,
                                    IdCashername = IdCasher,
                                    Cashername = CasherName,
                                    Quantity = _slthongbaochuyendoncuquadonmoi,
                                    DateCancel = DateTime.Now,
                                    IsSpitOrder = true,

                                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                    Note = $"Đã chuyển {_slthongbaochuyendoncuquadonmoi} {item.Name} sang  {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                };
                                lstdetail.Add(his);
                                //////


                              
                                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                                getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == kitchen.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                if (getitemnew == null)
                                {
                                    throw new Exception("Không tìm thấy item phù hợp");
                                }
                                var newitem = item.CloneJson();// phari clone chusw k sẽ bị update lại luôn item này vào db
                                newitem.Id = getitemnew.Id;
                                newitem.Quantity = _slthongbaochuyendoncuquadonmoi;
                                newitem.QuantityNotifyKitchen = _slthongbaochuyendoncuquadonmoi;
                                newitem.IdOrderTable = newOrder.Id;
                                newitem.Status = kitchen.Status;
                                lstOrderTableItemDonMoi.Add(newitem);



                                kitchen.Quantity = kitchen.Quantity - _slthongbaochuyendoncuquadonmoi;
                                updatekitchen = true;

                                // sau khi trừ xong thì phải thông báo cho đơn mới ở đây.
                              
                                break;

                            }
                            else if (kitchen.Quantity == _slthongbaochuyendoncuquadonmoi)
                            {

                               
                                //quantityCheck = 0;
                                var his = new DetailtKitchen()
                                {
                                    IdKitchen = kitchen.Id,
                                    IdCashername = IdCasher,
                                    Cashername = CasherName,
                                    Quantity = _slthongbaochuyendoncuquadonmoi,
                                    DateCancel = DateTime.Now,
                                    IsSpitOrder = true,

                                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                    Note = $"Được chuyển từ {(!string.IsNullOrEmpty(kitchen.RoomTableName) ? kitchen.RoomTableName : "Mang về")} mã đơn {kitchen.OrderCode} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                };
                                lstdetail.Add(his);
                                //////
                                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                                getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == kitchen.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                if (getitemnew == null)
                                {
                                    throw new Exception("Không tìm thấy item phù hợp");
                                }
                                kitchen.IdItemOrder = getitemnew.Id;
                                //----
                                kitchen.IdOrder = newOrder.IdGuid;
                                kitchen.IdRoomTable = newOrder.RoomAndTable?.IdGuid;
                                kitchen.RoomTableName = newOrder.RoomAndTable?.Name;
                                kitchen.Buyer = newOrder.Buyer;
                                kitchen.OrderCode = newOrder.OrderTableCode;
                                kitchen.IsBingBack = newOrder.IsBringBack;
                                updatekitchen = true;
                                break;
                            }
                            else if (kitchen.Quantity < _slthongbaochuyendoncuquadonmoi)
                            {
                                _slthongbaochuyendoncuquadonmoi = _slthongbaochuyendoncuquadonmoi - kitchen.Quantity;
                                // var historycu = new HistoryOrder()//lịch sử đơn cũ
                                //{
                                //    IdOrderTable = item.IdOrderTable,
                                //    IdProduct = item.IdProduct.Value,
                                //    Carsher = CasherName,
                                //    Code = randowm,
                                //    ProductName = item.Name,
                                //    IsNotif = true,
                                //    Quantity = _slthongbaochuyendoncuquadonmoi * -1,
                                //    CreateDate = DateTime.Now,
                                //    NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                                //    OrderCode = newOrder.OrderTableCode,
                                //    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                //    Name = $"Đã chuyển {_slthongbaochuyendoncuquadonmoi} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                //};
                                //lsthis.Add(history);

                                //quantityCheck = 0;
                                var his = new DetailtKitchen()
                                {
                                    IdKitchen = kitchen.Id,
                                    IdCashername = IdCasher,
                                    Cashername = CasherName,
                                    Quantity = _slthongbaochuyendoncuquadonmoi,
                                    DateCancel = DateTime.Now,
                                    IsSpitOrder = true,

                                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                    Note = $"Được chuyển từ {(!string.IsNullOrEmpty(kitchen.RoomTableName) ? kitchen.RoomTableName : "Mang về")} mã đơn {kitchen.OrderCode} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                                };
                                lstdetail.Add(his);
                                //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                                getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == kitchen.IdItemOrder).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                                if (getitemnew == null)
                                {
                                    throw new Exception("Không tìm thấy item phù hợp");
                                }
                                kitchen.IdItemOrder = getitemnew.Id;
                                //----

                                kitchen.IdOrder = newOrder.IdGuid;
                                kitchen.IdRoomTable = newOrder.RoomAndTable?.IdGuid;
                                kitchen.RoomTableName = newOrder.RoomAndTable?.Name;
                                kitchen.Buyer = newOrder.Buyer;
                                kitchen.OrderCode = newOrder.OrderTableCode;
                                kitchen.IsBingBack = newOrder.IsBringBack;
                                updatekitchen = true;
                            }
                            
                        }
                        if (updatekitchen)
                        {
                           await _kitchenRepository.UpdateRangeAsync(getitemtukitchen);
                        }

                    }
                    else if (quantitycheck> 0 && quantitycheck> getprnew.Quantity)//tức là số lượng còn  chưa thông báo của đơn cũ nó lớn hơn số lượng chọn tách qua đơn mới
                    {
                        //thông bsao cho đơn cũ với số lượng mới còn dư 
                        decimal slthongbaomoichodoncu = quantitycheck - getprnew.Quantity;//5-3(5 là chưa thông báo của đơn cũ, 3 là sl chuyển qua đon mới)
                        var _st = item.CloneJson();
                        _st.Quantity = slthongbaomoichodoncu;
                        _st.IdOrderTable = OrderOld.Id;
                        _st.Id = item.Id;
                        lstOrderTableItemDonCu.Add(_st);
                        // thông báo cho đơn cũ vs sl mới luôn
                        var history = new HistoryOrder()
                        {
                            IdOrderTable = OrderOld.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = slthongbaomoichodoncu,
                            CreateDate = today,
                            NewTableName = OrderOld.RoomAndTable != null ? OrderOld.RoomAndTable.Name : "mang về",
                            OrderCode = OrderOld.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.THEM,
                            Name = $"+ {_st.Quantity.ToString("0.###")} {item.Name}",
                        };
                        lsthis.Add(history);
                        // thông báo cho đơn mới sl còn lại getprnew.Quantity ví dụ quantitycheck còn 5 mà cần chuyenr 3 thôi
                        var listCHikencanbao = new List<OrderTableItem>();
                        //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                        var  getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == item.Id).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                        if (getitemnew == null)
                        {
                            throw new Exception("Không tìm thấy item phù hợp");
                        }
                        var _stt = item.CloneJson();
                        _stt.Id = getitemnew.Id;
                        _stt.Quantity = getprnew.Quantity;
                        _stt.IdOrderTable = newOrder.Id;
                        lstOrderTableItemDonMoi.Add(_stt);

                        // thông báo cho đơn mới vs sl mới luôn
                        var historymoi = new HistoryOrder()
                        {
                            IdOrderTable = newOrder.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = getprnew.Quantity,
                            CreateDate = today,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.THEM,
                            Name = $"+ {_st.Quantity.ToString("0.###")} {item.Name}",
                            // Name = $"Đã chuyển {quantityCheck} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                        };
                        lsthis.Add(historymoi);
                    }
                    else if (quantitycheck > 0 && quantitycheck == getprnew.Quantity)//th sluong còn chưa thông báo đơn cũ nó = vs sl cần chuyển qua đơn mới
                    {
                        //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                        var getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == item.Id).FirstOrDefault();//tìm id của đơn cũ đã chọn cần chuyển, update vào row chitken này với id mới
                        if (getitemnew == null)
                        {
                            throw new Exception("Không tìm thấy item phù hợp");
                        }
                        var _st = item.CloneJson();
                        _st.Id = getitemnew.Id;
                        _st.Quantity = quantitycheck;
                        _st.IdOrderTable = newOrder.Id;
                        lstOrderTableItemDonMoi.Add(_st);

                        var history2 = new HistoryOrder()
                        {
                            IdOrderTable = newOrder.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = _st.Quantity,
                            CreateDate = today,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Name = $"+{_st.Quantity.ToString("0.###")} {item.Name}"
                        };
                        lsthis.Add(history2);
                    }
                    else
                    {
                        
                    }
                    //trường họp bị xóa đi luôn nữa nhé tức là đảy qua đơn mới hết toàn bộ, sau đó check thêm đơn mới item đó có món mới k
                }
              
            }
            // ;lấy sản phẩm chuyển hết từ bàn cũ sang

            // lấy danh sách các sp đã chuyển qua hết bên đơn mới, OrderTableItemremove là sp bị chuyển hết sang đơn mới thì báo cho đơn cũ là đã chuyển qua đơn mới, và đơn mới là được chuyển
           
            foreach (var item in lstorderoldremove)
            {
                bool updatekitchen = false;
                //var getitemtukitchen = getDonCus.Where(x => x.IdProduct == item.IdProduct).ToList();
                var getitemtukitchen = getDonCus.Where(x => x.IdItemOrder == item.Id).ToList();
                var getitemsl = getitemtukitchen.Sum(x => x.Quantity);//lấy sl đã báo
                //kiểm tra sl đã báo
                if (item.Quantity >= item.QuantityNotifyKitchen)
                {
                    decimal quantitynew = item.Quantity - item.QuantityNotifyKitchen;
                    // for này là tìm tất cả dg có để chuyển qua đơn mới
                    foreach (var kitchen in getitemtukitchen.OrderBy(x => x.Status).ThenBy(x => x.Quantity))//lọc qua các item đã báo giảm đi đủ sl giảm, lọc từ mới và slnhor
                    {
                        //quantityCheck = 0;
                        var _historycu = new HistoryOrder()//lịch sử đơn cũ
                        {
                            IdOrderTable = item.IdOrderTable,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = kitchen.Quantity * -1,
                            CreateDate = DateTime.Now,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Name = $"Đã chuyển {kitchen.Quantity.ToString("0.###")} {item.Name} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                        };
                        lsthis.Add(_historycu);
                        //chi tiết báo bếp update lại cái báo cho đơn cũ thì chi tiết là báo chuyển qua đơn mới
                        var his = new DetailtKitchen()
                        {
                            IdKitchen = kitchen.Id,
                            IdCashername = IdCasher,
                            Cashername = CasherName,
                            Quantity = kitchen.Quantity,
                            DateCancel = DateTime.Now,
                            IsSpitOrder = true,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Note = $"Được chuyển từ {(!string.IsNullOrEmpty(kitchen.RoomTableName) ? kitchen.RoomTableName : "Mang về")} mã đơn {kitchen.OrderCode} sang {(newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về")} mã đơn {newOrder.OrderTableCode}",
                        };
                        lstdetail.Add(his);
                        //----lịch sử cho đơn mới sau khi được chuyển từ đơn cũ qua
                        var history = new HistoryOrder()
                        {
                            IdOrderTable = newOrder.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = kitchen.Quantity,
                            CreateDate = today,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                            Name = $"+ {kitchen.Quantity.ToString("0.###")} {item.Name} (được chuyển từ {(!string.IsNullOrEmpty(kitchen.RoomTableName) ? kitchen.RoomTableName : "Mang về")} sang)",
                        };
                        lsthis.Add(history);
                        //end
                        //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                        var getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == item.Id).FirstOrDefault();
                        if (getitemnew == null)
                        {
                            throw new Exception("Không tìm thấy sản phẩm");
                        }
                        kitchen.IdItemOrder = getitemnew.Id;
                        //----
                        kitchen.IdOrder = newOrder.IdGuid;
                        kitchen.IdRoomTable = newOrder.RoomAndTable?.IdGuid;
                        kitchen.RoomTableName = newOrder.RoomAndTable?.Name;
                        kitchen.Buyer = newOrder.Buyer;
                        kitchen.OrderCode = newOrder.OrderTableCode;
                        kitchen.IsBingBack = newOrder.IsBringBack;
                        updatekitchen = true;
                        break;
                    }
                    if (quantitynew > 0)//số lượng còn lại này là số lượng chưa báo bếp mà chuyển sang bàn mới nên sẽ thông báo cho bàn mới
                    {
                        var _nitem = item.CloneJson();
                        //----tìm đúng hàng hóa đó đúng sl để update luôn id item,để tìm kiếm dc
                        var getitemnew = newOrder.OrderTableItems.Where(z => z.IdItemOrderOld == item.Id).FirstOrDefault();
                        if (getitemnew == null)
                        {
                            throw new Exception("Không tìm thấy sản phẩm");
                        }
                        _nitem.Id = getitemnew.Id;
                        _nitem.Quantity = quantitynew;
                        _nitem.IdOrderTable = newOrder.Id;
                        lstOrderTableItemDonMoi.Add(_nitem);
                        //lịch sử cho nó đơn mới
                        var history = new HistoryOrder()
                        {
                            IdOrderTable = newOrder.Id,
                            IdProduct = item.IdProduct.Value,
                            Carsher = CasherName,
                            Code = randowm,
                            ProductName = item.Name,
                            IsNotif = true,
                            Quantity = _nitem.Quantity,
                            CreateDate = today,
                            NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                            OrderCode = newOrder.OrderTableCode,
                            TypeKitchenOrder = EnumTypeKitchenOrder.THEM,
                            Name = $"+ {_nitem.Quantity.ToString("0.###")} {item.Name}",
                        };
                        lsthis.Add(history);
                    }
                }
                if (updatekitchen)
                {
                    await _kitchenRepository.UpdateRangeAsync(getitemtukitchen);
                }
            }

            // lọc đơn mới xem có sp nào chưa báo bếp thì báo vì lúc tách chưa bấm báo
          
            foreach (var item in newOrder.OrderTableItems)
            {
                if (item.Quantity > item.QuantityNotifyKitchen)
                {
                    var _nitem = item.CloneJson();
                    _nitem.Quantity = item.Quantity - item.QuantityNotifyKitchen;
                    _nitem.Id = item.Id;
                    _nitem.IdOrderTable = newOrder.Id;
                    lstOrderTableItemDonMoi.Add(_nitem);
                    item.QuantityNotifyKitchen = item.Quantity;

                    //lịch sử cho nó đơn mới
                    var history = new HistoryOrder()
                    {
                        IdOrderTable = newOrder.Id,
                        IdProduct = item.IdProduct.Value,
                        Carsher = CasherName,
                        Code = randowm,
                        ProductName = item.Name,
                        IsNotif = true,
                        Quantity = _nitem.Quantity,
                        CreateDate = today,
                        NewTableName = newOrder.RoomAndTable != null ? newOrder.RoomAndTable.Name : "mang về",
                        OrderCode = newOrder.OrderTableCode,
                        TypeKitchenOrder = EnumTypeKitchenOrder.THEM,
                        Name = $"+ {_nitem.Quantity.ToString("0.###")} {item.Name}",
                    };
                    lsthis.Add(history);
                }
            }
            if (lsthis.Count() > 0)
            {

                await _historyOrderRepository.AddHistoryOrder(lsthis);
            }
            if (lstdetail.Count() > 0)
            {
                await _detailtKitchenRepository.AddRangeAsync(lstdetail);
            }
            if (lstOrderTableItemDonCu.Count() > 0)
            {
                await this.AddNotify(lstOrderTableItemDonCu, OrderOld);
            }
            if (lstOrderTableItemDonMoi.Count() > 0)
            {
                await this.AddNotify(lstOrderTableItemDonMoi, newOrder);


                ////----lọc ra các đơn mà chưa thông báo lần nào mà có chứa ghi chú báo riêng.
                //var lstghichu = lstOrderTableItemDonMoi.Where(x=>x.QuantityNotifyKitchen==0&& !string.IsNullOrEmpty(x.Note)).ToList();
                //if (lstghichu.Count()>0)
                //{
                //    await this.AddNotify(lstghichu, newOrder);
                //    lstghichu.ForEach(x => lstOrderTableItemDonMoi.Remove(x));
                //}

                //if (lstOrderTableItemDonMoi.Count()>0)//xem danh sách còn lại sau khi lọc lấy ghi chú, nếu còn thì báo
                //{
                //    var newlst = lstOrderTableItemDonMoi.Where(x => !lstghichu.Select(x => x.Id).ToArray().Contains(x.Id)).GroupBy(x => new { x.IdOrderTable, x.Id, x.Status }).Select(x => new OrderTableItem()
                //    {
                //        IdOrderTable = x.Key.IdOrderTable,
                //        Quantity = x.Sum(z => z.Quantity),
                //        IdProduct = x.First().IdProduct,
                //        Name = x.First()?.Name,
                //        Code = x.First()?.Code,
                //        Status = x.Key.Status,
                //        QuantityNotifyKitchen = x.Sum(z => z.Quantity)
                //    }).ToList();

                //    await this.AddNotify(newlst, newOrder);
                //}
            }
        }

        public async Task UpdateNotifyAllByRoomTable(int Comid, OrderTable order, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC)
        {
            var getDonCu = _kitchenRepository.Entities.Where(x => x.ComId == Comid && x.IdOrder == order.IdGuid && x.IdDichVu== IdDichVu).ToList();
            if (order.IsBringBack)
            {
                getDonCu.ForEach(x => { x.IdRoomTable = null;x.IsBingBack = order.IsBringBack; x.RoomTableName = "Mang về"; });
            }
            else
            {
                getDonCu.ForEach(x => { x.IdRoomTable = order.IdRoomAndTableGuid; x.IsBingBack = order.IsBringBack; x.RoomTableName = order.RoomAndTable?.Name; });
            }
            await _kitchenRepository.UpdateRangeAsync(getDonCu);
        }

        public async Task<KitChenTableModel> GetAllNotifyOrderByTable(int Comid)
        {
            chưa lấy dc dữ liệu
            //lấy dữ liệu
            var query = await _ordertableRepository.Entities.Where(x=>x.ComId==Comid&&x.Status==EnumStatusOrderTable.DANG_DAT).Join(_kitchenRepository.Entities.Where(x=>x.ComId== Comid),
                        ordertable => ordertable.IdGuid,
                        kitchen => kitchen.IdRoomTable,
                        (ordertable, kitchen) => new OrderDetailByOrder
                        {
                            orderCode = kitchen.OrderCode,
                            quantity = kitchen.Quantity,
                            Note = kitchen.Note,
                            orderStaff = kitchen.Cashername,
                            idRoomTable = kitchen.IdRoomTable,
                            idOrder = kitchen.IdOrder,
                            IdItemOrder = kitchen.IdItemOrder,
                            idProduct = kitchen.IdProduct,
                            tableName =kitchen.RoomTableName,
                            proName = kitchen.ProName,
                            proCode = kitchen.ProCode,
                            createDate = ordertable.CreatedOn,
                            createDateTable = ordertable.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                            createDateFood = kitchen.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                        }).Where(x=>x.quantity>0).ToListAsync();
            List< KitChenTableListModel> kitChenlistbyTableModel = new List<KitChenTableListModel>();
            //------xử lý gr theo bàn
            var grbytable = query.GroupBy(x => x.idRoomTable);
            foreach (var item in grbytable)
            {
                TimeSpan usedTime = DateTime.Now.Subtract(item.First().createDate);
                kitChenlistbyTableModel.Add(new KitChenTableListModel()
                {
                    idRoomTable = item.Key,
                    tableName = item.First().tableName,
                    createDateTable = item.First().createDateTable,
                    TimeSpan = usedTime.TotalSeconds,
                    quantity = item.Sum(x => x.quantity),
                    OrderDetailByOrders = item.Select(x => new OrderDetailByOrder()
                    {
                        quantity = x.quantity,
                        proName = x.proName,
                        IdItemOrder = x.IdItemOrder,
                        idRoomTable = item.Key,
                        idProduct = x.idProduct,
                        idOrder = x.idOrder,
                        Note = x.Note,
                        createDateFood = x.createDateFood,
                        tableName = x.tableName,
                        orderStaff = x.orderStaff,
                    }).ToList()
            });
              

            }
            //------nhóm theo bàn end
            //--nhóm theo sản phẩm
            List<OrderByFoodModel> orderByFoodModels = new List<OrderByFoodModel>();    
            var grbyfood= query.GroupBy(x => x.idProduct);
            foreach (var item in grbyfood)
            {
                orderByFoodModels.Add(new OrderByFoodModel()
                {
                    idProduct = item.Key,
                    proName = item.First().proName,
                    quantity = item.Sum(x=>x.quantity),
                    note = string.Join(",", item.Select(x => x.Note).ToArray()),
                    OrderDetailByOrders = item.Select(x=>new OrderDetailByOrder()
                    {
                        quantity = x.quantity,
                        proName = x.proName,
                        idOrder = x.idOrder,
                        IdItemOrder = x.IdItemOrder,
                        Note = x.Note,
                        tableName = x.tableName,
                        orderStaff = x.orderStaff,
                        createDateFood = x.createDateFood,
                        createDateTable = x.createDateTable,
                        orderCode = x.orderCode,

                    }).ToList()
                });
            }
            //--
            KitChenTableModel kitChenTableModel = new KitChenTableModel();
            kitChenTableModel.KitChenTableListModels = kitChenlistbyTableModel;
            kitChenTableModel.OrderByFoodModels = orderByFoodModels;
            return kitChenTableModel;
        }

        //public Task UpdateNotifyKitchenSpitOrderIsCreateOrderAsync(Guid IdOrder, List<OrderTableItem> lstorderold, int ComId, OrderTable newOrder)
        //{
        //    var getDonCu = _kitchenRepository.Entities.Where(x => x.ComId == ComId && x.IdOrder == IdOrder && !x.IsCancelAll).ToList();// tất cả thông báo của đơn này


        //}
    }
}
