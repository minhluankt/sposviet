using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography;
using System.Net;
using SposVietPluginKySo.Model;
using Newtonsoft.Json;
using static SposVietPluginKySo.Enum;
using SposVietPluginKySo.Service;
using System.Diagnostics;

namespace SposVietPluginKySo
{

    //https://github.com/ParametricCamp/TutorialFiles/tree/master/Misc/WebSockets

    //https://stackoverflow.com/questions/30523478/connecting-to-websocket-using-c-sharp-i-can-connect-using-javascript-but-c-sha
    public  class WebSocketSharpSposViet
    {
        public static WebSocketServer wssv;
        private static bool IsCreated;
        public static void StartWebSocket()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
           
            try{
                if (wssv == null || !IsCreated)
                {
                    var gepath = wssv?.WebSocketServices?.Paths?.FirstOrDefault(x => x.Contains("8056"));
                    if (gepath==null)
                    {
                        wssv = new WebSocketServer(8056);
                        //wssv = new WebSocketServer("wss://127.0.0.1:8056");
                        wssv.AddWebSocketService<Echo>("/SposVietPlugin");
                        // wssv.AddWebSocketService<EchoAll>("/EchoAll");//dùng cho tất cả người dùng
                        wssv.Start();
                        IsCreated = true;
                       // wssv.Stop();
                    }
                   
                }
                
            }
            catch (WebSocketException wse)
            {
                wssv = null;
                Thread.Sleep(3000);
                WebSocketSharpSposViet.StartWebSocket();
            }
            catch (Exception e)
            {
                wssv = null;
                Thread.Sleep(3000);
                WebSocketSharpSposViet.StartWebSocket();
                //wssv = null;
                //IsCreated = false;
                //var action = new Action(() => {
                //    WebSocketSharpSposViet.StartWebSocket();
                //});
                //new Echo().SetTimeout(action, 1000); //đóng conncet thì tự gọi lại sau 3 giây
            }
        }
    }
    public  class Echo : WebSocketBehavior
    {
        public  void SetTimeout(Action action, int timeout)
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = timeout;
            timer.Tick += (s, e) =>
            {
                action();
                timer.Stop();
            };
            timer.Start();
        }
        //protected override void OnOpen()
        //{

        //https://stackoverflow.com/questions/71007077/only-one-usage-of-each-socket-address-protocol-network-address-port-is-normall
        //}
        protected override void OnClose(CloseEventArgs e)
        {
            //if (WebSocketSharpSposViet.wssv != null)
            //{
            //    WebSocketSharpSposViet.wssv.Stop();
            //    Thread.Sleep(1000);
            //    removeWebSocketServices();
            //    WebSocketSharpSposViet.wssv = null;
            //}
            //Thread.Sleep(1000);
            //var action = new Action(() => {
            //    WebSocketSharpSposViet.StartWebSocket();
            //});
            //SetTimeout(action, 300); //đóng conncet thì tự gọi lại sau 3 giây
            //WebSocketSharpSposViet.StartWebSocket();
        }
        internal static void removeWebSocketServices()
        {
            var channels = WebSocketSharpSposViet.wssv?.WebSocketServices?.Paths?.ToList() ?? null;
            if (channels != null)
            {
                channels.ForEach(channel => { WebSocketSharpSposViet.wssv.RemoveWebSocketService(channel); channel = null; });
            }
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {

        }

        protected override void OnMessage(MessageEventArgs e)//khi nhận dữ liệu gửi từ client
        {
            // Console.WriteLine("Received message from Echo client: " + e.Data);
            try
            {
                var res = new ResponseModel<string>();
                string messageToFatima = e.Data;
                var model = JsonConvert.DeserializeObject<DataSignEinvoiceModel>(e.Data);
                
                if (model != null)
                {
                    res.type = (int)model.TypeEventWebSocket;
                    switch (model.TypeEventWebSocket)
                    {
                        case TypeEventWebSocket.SendTestConnect:
                            res.Message = "Test thành công";
                            res.isSuccess = true;
                            string data = JsonConvert.SerializeObject(res);
                            Send(data);
                            break;
                        case TypeEventWebSocket.SignEInvoice:
                            if (string.IsNullOrEmpty(model.hash))
                            {
                                res.isSuccess = false;
                                res.Message = "Không tồn tại hash";
                                Send(JsonConvert.SerializeObject(res));
                            }
                            else {
                                EInvoiceService eInvoiceService = new EInvoiceService();
                                string sign = eInvoiceService.SignHashEInvoiceToken(model.hash);
                                if (sign=="-1")
                                {
                                    res.isSuccess = false;
                                    res.Message = "-1";
                                    res.Data = sign;
                                    Send(JsonConvert.SerializeObject(res));
                                }
                                else
                                {
                                    res.isSuccess = true;
                                    res.Message = "Ký số thành công";
                                    res.Data = sign;
                                    Send(JsonConvert.SerializeObject(res));
                                }
                              
                            }
                            
                            break;
                        case TypeEventWebSocket.PrintEInvoice:
                            if (string.IsNullOrEmpty(model.html))
                            {
                                res.isSuccess = false;
                                res.Message = "Không tồn tại data";
                                Send(JsonConvert.SerializeObject(res));
                            }
                            else
                            {
                                PrintServer.Print(model.html);
                                res.isSuccess = true;
                                res.Message = "In thành công";
                                Send(JsonConvert.SerializeObject(res));
                            }

                            break; 
                        case TypeEventWebSocket.PrintBep:
                            if (string.IsNullOrEmpty(model.html))
                            {
                                res.isSuccess = false;
                                res.Message = "Không tồn tại data";
                                Send(JsonConvert.SerializeObject(res));
                            }
                            else
                            {
                                LogControl.Write("Bắt đầu in wss thành công");
                                PrintServer.PrintPageBaoBep(model.html);
                                res.isSuccess = true;
                                res.Message = "In thành công";
                                Send(JsonConvert.SerializeObject(res));
                                LogControl.Write("in thành công");
                            }

                            break;
                        case TypeEventWebSocket.PrintInvoice:
                            if (string.IsNullOrEmpty(model.html))
                            {
                                res.isSuccess = false;
                                res.Message = "Không tồn tại data";
                                Send(JsonConvert.SerializeObject(res));
                            }
                            else
                            {
                                PrintServer.Print(model.html);
                                res.isSuccess = true;
                                res.Message = "In thành công";
                                Send(JsonConvert.SerializeObject(res));
                            }

                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    res = new ResponseModel<string>() { Data = "", isSuccess = false, Message = "Lỗi không tìm thấy dữ liệu" };
                    string data = JsonConvert.SerializeObject(res);
                    Send(data);
                }
            }
            catch (Exception ex)
            {
                var res = new ResponseModel<string>() { Data = "", isSuccess = false, Message = "Lỗi không tìm thấy dữ liệu" + ex.Message };
                string data = JsonConvert.SerializeObject(res);
                Send(data);
            }
        }
    }

    public class EchoAll : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Received message from EchoAll client: " + e.Data);
            Sessions.Broadcast(e.Data);//cho tất cả client
        }
    }
}
