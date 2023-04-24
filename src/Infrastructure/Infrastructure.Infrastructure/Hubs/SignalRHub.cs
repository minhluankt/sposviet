using Application.Enums;
using Application.Hepers;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.HubS
{
    [AllowAnonymous]
    public class SignalRHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;
        public SignalRHub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public static List<KeyValuePair<string, string>> Ids = new List<KeyValuePair<string, string>>();
        public override async Task OnConnectedAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                //var currentUser = await _userManager.GetUserAsync(Context.User);
                //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var currentUser = Context.User.Identity.GetUserClaimLogin();
                if (currentUser != null)
                {
                    string _Groupbep = $"{currentUser.ComId}_CHITCHEN";
                    checkExitRoomChitchen(Context.ConnectionId, _Groupbep);

                    string _Group = $"{currentUser.ComId}_POS";
                    checkExitRoomChitchen(Context.ConnectionId, _Group);
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
        public async Task Printbaobep(string data,string text= "IN")
        {
            var currentUser = Context.User.Identity.GetUserClaimLogin();
            string _Group = $"{currentUser.ComId}_Printbaobep";
            checkExitRoomChitchen(Context.ConnectionId, _Group);
            //await Clients.OthersInGroup(_Group).SendAsync("Printbaobep", data);
            if (text=="TEST")
            {
                await Clients.Groups(_Group).SendAsync("Printbaobep", text);
            }
            else
            {
                await Clients.Groups(_Group).SendAsync("Printbaobep", data);
            }
        }
        public async Task PrintbaobepSposViet(int ComId, string data)//dành cho báo  bếp tại máy khashc hàng 
        {
            var json = new
            {
                Type = data=="TEST"? EnumTypePrint.TEST: EnumTypePrint.PrintBaoBep,
                ComId = ComId,
                data = data,
            };
            string datajson = Common.ConverModelToJson(json);
            string _Group = $"{ComId}_Printbaobep";
            checkExitRoomChitchen(Context.ConnectionId, _Group);
            await Clients.Groups(_Group).SendAsync("PrintbaobepSposViet", datajson);//PrintbaobepSposViet là hàm ở client nhận method
        }
        public async Task sendNotifyPos(EnumTypeSignalRHub STATUS, EnumTypeSignalRHub type, string note = "")
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                //var currentUser = await _userManager.GetUserAsync(Context.User);
                // var currentUser = await _userManager.GetUserAsync(Context.User);
                var currentUser = Context.User.Identity.GetUserClaimLogin();
                if (STATUS == EnumTypeSignalRHub.POS && type == EnumTypeSignalRHub.POS)
                {
                    string _Group = $"{currentUser.ComId}_POS";
                    if (!Ids.Where(x => x.Key == Context.ConnectionId && x.Value == _Group).Any())
                    {
                        Ids.Add(new KeyValuePair<string, string>(Context.ConnectionId, _Group));
                    }

                    await Groups.AddToGroupAsync(Context.ConnectionId, _Group);

                    bool isValid = true;
                    await Clients.OthersInGroup(_Group).SendAsync("addMessagePOS", isValid); // OthersInGroup là báo cho các người trong nhóm trừ thèn dg gọi,GroupExcept là trừ chỉ định

                    /// thông báo cho bếp khi cấm thông báo
                    if (type == EnumTypeSignalRHub.POS)
                    {
                        string _Groupbep = $"{currentUser.ComId}_CHITCHEN";
                        if (!Ids.Where(x => x.Key == Context.ConnectionId && x.Value == _Groupbep).Any())
                        {
                            Ids.Add(new KeyValuePair<string, string>(Context.ConnectionId, _Groupbep));
                        }
                        await Groups.AddToGroupAsync(Context.ConnectionId, _Groupbep);
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
        private async void checkExitRoomChitchen(string ConnectionId, string _Group)
        {
            if (Ids.Count()>0)
            {
                if (!Ids.Where(x => x.Key == ConnectionId && x.Value == _Group).Any())
                {
                    Ids.Add(new KeyValuePair<string, string>(Context.ConnectionId, _Group));
                    await Groups.AddToGroupAsync(Context.ConnectionId, _Group);
                }
            }
            else
            {
                Ids.Add(new KeyValuePair<string, string>(Context.ConnectionId, _Group));
                await Groups.AddToGroupAsync(Context.ConnectionId, _Group);
            }
        }

    }
}
