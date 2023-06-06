using Application.Enums;
using Application.Hepers;
using Domain.Identity;
using FluentValidation.Results;
using Hangfire.Storage;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Infrastructure.HubS
{

    [AllowAnonymous]
    public class SignalRHub : Hub
    {
        private bool isreconncet = false;
        private readonly IHttpContextAccessor _httpcontext;
        private readonly ILogger<SignalRHub> _log;
        private readonly IServiceProvider _serviceProvider;
        public SignalRHub(IServiceProvider serviceProvider, ILogger<SignalRHub> log, IHttpContextAccessor httpcontext)
        {
            _log = log;
            _serviceProvider = serviceProvider;
            _httpcontext = httpcontext;
        }
        public static List<KeyValuePair<string, string>> Ids = new List<KeyValuePair<string, string>>();
        //https://www.hieutech.vn/2022/09/gioi-thieu-ve-realtime-voi-signalr-trong-net-core.html
        public override async Task OnConnectedAsync()
        {
            //using (var scope = _serviceProvider.CreateScope())
            {
               // var _userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                //var currentUser = await _userManager.GetUserAsync(Context.User);
                //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var currentUser = Context.User.Identity.GetUserClaimLogin();
                if (currentUser != null)
                {
                    isreconncet = true;
                    if (currentUser.IsBep || currentUser.IsAdmin)
                    {
                        string _Groupbep = $"{currentUser.ComId}_CHITCHEN";
                        checkExitRoomChitchen(Context.ConnectionId, _Groupbep);
                    }
                    if (currentUser.IsPhucVu || currentUser.IsAdmin|| currentUser.IsBep)
                    {
                        string _Groupbep = $"{currentUser.ComId}_StaffAlertBep";
                        checkExitRoomChitchen(Context.ConnectionId, _Groupbep);
                    }
                    if (currentUser.IsThuNgan || currentUser.IsAdmin)
                    {
                        string _Group = $"{currentUser.ComId}_POS";
                        checkExitRoomChitchen(Context.ConnectionId, _Group);
                        await this.LoadOrdertable(EnumTypePrint.TEST, "reconcect LoadOrdertable");
                    }
                   
                    await Printbaobep("reconcect Printbaobep", "TEST");
                    await PrintbaobepSposViet(currentUser.ComId, "TEST");
                    isreconncet = false;
                }
                await base.OnConnectedAsync();
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var getitem = Ids.Where(x => x.Key == Context.ConnectionId).ToList();
            if (getitem != null)
            {
                foreach (var item in getitem)
                {
                    Ids.Remove(item);
                    await Groups.RemoveFromGroupAsync(item.Key, item.Value);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Send(EnumTypeSignalRHub STATUS, string message)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var _userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));

               // var currentUser = await _userManager.GetUserAsync(Context.User);
               
                var currentUser = Context.User.Identity.GetUserClaimLogin();
                //if (await _userManager.IsInRoleAsync(currentUser, "PHỤC VỤ"))
                //{

                //}
                string _Group = $"{currentUser.ComId}_POS";
                await Groups.AddToGroupAsync(Context.ConnectionId, _Group);
                int numberRetry2 = 0;
            RetryInvoice2:
                // message = numberRetry2 + "__" + test.connect(Context.ConnectionId);
                //await Clients.Caller.SendAsync("broadcastMessage", name, message);//chính thèn dg tương tác
                bool isValid = true;
                await Clients.OthersInGroup(_Group).SendAsync("broadcastMessage", isValid); // OthersInGroup là báo cho các người trong nhóm trừ thèn dg gọi,GroupExcept là trừ chỉ định
                                                                                            // bool checkuser = Ids.Contains(Context.ConnectionId);
            }

            //await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{name}");
            // Call the broadcastMessage method to update clients.

            //if (numberRetry2 < 20 && checkuser)
            //{
            //    numberRetry2++;
            //    Thread.Sleep(1000);
            //    goto RetryInvoice2;
            //}

            // await Clients.All.SendAsync("broadcastMessage", name, message);
        }
        public async Task Printbaobep(string data,string text= "IN")//dành cho phương án 1
        {
            var currentUser = Context.User.Identity.GetUserClaimLogin();
            if (currentUser!=null)
            {
                string _Group = $"{currentUser.ComId}_Printbaobep";
                checkExitRoomChitchen(Context.ConnectionId, _Group);
                //await Clients.OthersInGroup(_Group).SendAsync("Printbaobep", data);
                if (text == "TEST")
                {
                    await Clients.Group(_Group).SendAsync("Printbaobep", text);
                }
                else
                {
                    await Clients.Group(_Group).SendAsync("Printbaobep", data);
                    //try
                    //{
                    //   await this.PrintbaobepSposViet(currentUser.ComId, data);
                    //}
                    //catch (Exception e)
                    //{
                    //    _log.LogInformation("PrintbaobepSposViet thất bại");
                    //    _log.LogInformation(e.ToString());
                    //}
                }
            }
            
        }
        public async Task LoadOrdertable(EnumTypePrint Type,string JsonData)
        {
            
            var currentUser = _httpcontext.HttpContext.User.Identity.GetUserClaimLogin();
            if (currentUser!=null)
            {
                string _Group = $"{currentUser.ComId}_LoadOrdertable";
                var data = new
                {
                    user = currentUser.UserName,
                    type = Type,
                    data = JsonData
                }; string datajson = Common.ConverModelToJson(data);
                checkExitRoomChitchen(this.Context.ConnectionId, _Group);
                // checkExitRoomChitchen(currentUser.Id, _Group);
                await Clients.Group(_Group).SendAsync("LoadOrdertable", datajson);
                //if (isreconncet)
                //{
                //    await Clients.Group(_Group).SendAsync("LoadOrdertable", datajson);
                //}
                //else
                //{
                //    await Clients.Group(_Group).SendAsync("LoadOrdertable", datajson);//trừ thèn dg nhấn
                //}
            }
            else
            {

                _log.LogError("Không tìm thấy user để load");
            }
            
        }
        public async Task một hàm cập nhập dữ liệu
        public async Task PrintbaobepSposViet(int ComId, string data)//dành cho báo  bếp tại máy khashc hàng phương án 2 k cần bật ứng dụng web, chỉ cần tool
        {
            var json = new
            {
                Type = data=="TEST"? EnumTypePrint.TEST: EnumTypePrint.PrintBaoBep,
                ComId = ComId,
                data = data,
            };
            string datajson = Common.ConverModelToJson(json);
            string _Group = $"{ComId}_PrintbaobepSposViet";
            checkExitRoomChitchen(Context.ConnectionId, _Group);
            await Clients.Group(_Group).SendAsync("PrintbaobepSposViet", datajson);//PrintbaobepSposViet là hàm ở client nhận method
        }
       // thực hiện chỉnh sadmin công ty hiển thị cột comid, lưu ở tool cho phép gọi api lấy dữ liệu, sau đó test nhé
        public async Task sendNotifyPos(EnumTypeSignalRHub STATUS, EnumTypeSignalRHub type, string note = "")
        {
            //using (var scope = _serviceProvider.CreateScope())
            {
                // var _userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));

                var currentUser = _httpcontext.HttpContext.User.Identity.GetUserClaimLogin();
                if (currentUser!=null)
                {
                    if (STATUS == EnumTypeSignalRHub.POS && type == EnumTypeSignalRHub.POS)
                    {
                        string _Group = $"{currentUser.ComId}_POS";
                       
                        checkExitRoomChitchen(Context.ConnectionId, _Group);

                        bool isValid = true;
                        await Clients.OthersInGroup(_Group).SendAsync("addMessagePOS", isValid); // OthersInGroup là báo cho các người trong nhóm trừ thèn dg gọi,GroupExcept là trừ chỉ định

                        /// thông báo cho bếp khi cấm thông báo
                        if (type == EnumTypeSignalRHub.POS)
                        {
                            string _Groupbep = $"{currentUser.ComId}_CHITCHEN";
                            checkExitRoomChitchen(Context.ConnectionId, _Groupbep); 
                            await Clients.OthersInGroup(_Group).SendAsync("addMessageCHITKEN", isValid);
                        }

                    }
                    else if (STATUS == EnumTypeSignalRHub.CHITKEN && type == EnumTypeSignalRHub.KITCHENTOPOS)// KHI BẤm bếp sang ready thông báo cho pos thu ngân biết
                    {
                        string _Group = $"{currentUser.ComId}_POS";
                        checkExitRoomChitchen(Context.ConnectionId, _Group);
                        await Clients.Group(_Group).SendAsync("addMessageCHITKENBYPOS", note);
                    }
                    else if (STATUS == EnumTypeSignalRHub.CHITKEN && type == EnumTypeSignalRHub.CHITKEN)// thông báo từ thu ngân qua bếp
                    {
                        string _Group = $"{currentUser.ComId}_CHITCHEN";
                        checkExitRoomChitchen(Context.ConnectionId, _Group);
                        //bool checkuser = Ids.Contains(Context.ConnectionId);
                        //await Groups.AddToGroupAsync(Context.ConnectionId, _Group);
                        bool isValid = true;//là k cần thông báo chuông
                        await Clients.Group(_Group).SendAsync("addMessageCHITKEN", isValid);
                    }
                    else if (STATUS == EnumTypeSignalRHub.CHITKEN && (type == EnumTypeSignalRHub.UPDATECHITKEN || type == EnumTypeSignalRHub.DELETECHITKEN))//cái này là lúc bấm chuyển từ bếp đang nấu sang ready
                    {
                        string _Group = $"{currentUser.ComId}_CHITCHEN";
                        checkExitRoomChitchen(Context.ConnectionId, _Group);
                        //bool checkuser = Ids.Contains(Context.ConnectionId);

                        bool isValid = false;//là k cần thông báo
                        await Clients.Group(_Group).SendAsync("addMessageCHITKEN", isValid);
                    }
                }
               
            }
        }
        //-------------xử lý cho nhân viên yêu cầu hủy món
        public async Task StaffAlertBep(int ComId, string data)// tên StaffAlertBep là đùng dể invoke ở client
        {
            if (data == "TEST")
            {
                string _Group = $"{ComId}_StaffAlertBep";
                checkExitRoomChitchen(Context.ConnectionId, _Group);
                await Clients.Group(_Group).SendAsync("StaffAlertBep", data);//StaffAlertBep là hàm ở client nhận method
            }
            else
            {
                var json = new
                {
                    //Type = data == "TEST" ? EnumTypePrint.TEST : EnumTypePrint.PrintBaoBep,
                    ComId = ComId,
                    data = data,
                };
                string datajson = Common.ConverModelToJson(json);
                string _Group = $"{ComId}_StaffAlertBep";
                checkExitRoomChitchen(Context.ConnectionId, _Group);
                await Clients.Group(_Group).SendAsync("StaffAlertBep", datajson);//StaffAlertBep là hàm ở client nhận method on
            }
           
            
        }
        //---------------
        //------xử lý khi bếp phản hồi lại cho nhân viên phục vụ
        public async Task FeedbackBepToStaff(string userId, string data, int type)// tên FeedbackBepToStaff là đùng dể invoke ở client
        {
            // type 1 là yêu cầu trùng vì đã có nhân viên trước đó đã thao tác gửi yêu cầu, chặn k cho 2nguowfi thao tác trên 1 bàn
            // type 2 gủi data xác nhận đồng ý hủy món
            // type 3 gủi data xác nhận từ chối hủy món
            var currentUser = Context.User.Identity.GetUserClaimLogin();
            if (data == "TEST")
            {
                string _Group = $"{currentUser.ComId}_FeedbackBepToStaff";
                checkExitRoomChitchen(Context.ConnectionId, _Group);
                await Clients.Group(_Group).SendAsync("FeedbackBepToStaff", data);//FeedbackBepToStaff là hàm ở client nhận method
            }
            else
            {
                if (type==1)
                {
                    var json = new
                    {
                        IsValid = false,
                        Mess = data,
                    };
                    await Clients.User(userId).SendAsync("FeedbackBepToStaff", Common.ConverModelToJson(json));//StaffAlertBep là hàm ở client nhận method on, gủi cho nhân viên là k dc gửi yêu cầu cùng bàn
                }
                else if (type==2)
                {

                }
            }
        }
        //----------
        private async void checkExitRoomChitchen(string ConnectionId, string _Group)
        {
            if (Ids.Count()>0)
            {
                if (!Ids.Where(x => x.Key == ConnectionId && x.Value == _Group).Any())
                {
                    Ids.Add(new KeyValuePair<string, string>(ConnectionId, _Group));
                    await Groups.AddToGroupAsync(ConnectionId, _Group);
                }
            }
            else
            {
                Ids.Add(new KeyValuePair<string, string>(ConnectionId, _Group));
                await Groups.AddToGroupAsync(ConnectionId, _Group);
            }
        }

    }
}
