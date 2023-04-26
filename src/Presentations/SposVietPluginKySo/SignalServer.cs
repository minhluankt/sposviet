using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Owin;
using SignalR.Hosting.Self;
using SposVietPluginKySo.Model;

namespace SposVietPluginKySo
{
    public class SignalServer
    {//https://stackoverflow.com/questions/64551450/how-to-make-a-signalr-chat-in-winforms
     //https://stackoverflow.com/questions/11140164/signalr-console-app-example
        //public  void Start()
        //{
        //    var url = "https://fnb.sposviet.vn";
        //    //using (WebApp.Start<Startup>(new StartOptions(url: url)
        //    //{
        //    //    Port = 8323,
        //    //    ServerFactory = "Microsoft.Owin.Host.HttpListener"
        //    //}))
        //    //{
        //    //    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        //    //    context.Clients.All.addMessage("Server", "Hello from server");
        //    //}

        //    using (WebApp.Start<Startup>(url))
        //    {
        //        // Send message to all connected clients
        //        var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        //        context.Clients.All.addMessage("Server", "Hello from server");
        //        //  Console.WriteLine($"Server running at {url}");
        //        // Console.ReadLine();
        //    }
        //}
        //public  void StartSignalServer()
        //{
        //    //chạy loca
        //    var getcomid = Properties.Settings.Default.ComId;
        //    var url = "https://fnb.sposviet.vn/";
        //    //var url = $"https://localhost:7269/{getcomid}SposVietPrint";
        //    var server = new Server(url);
        //    // Map the default hub url (/signalr)
        //    server.MapHubs($"/{getcomid}SposVietPrint");
        //    // Start the server
        //    server.Start();
        //    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        //    context.Clients.All.addMessage("Server", "Hello from server");
        //}

        static Microsoft.AspNet.SignalR.Client.HubConnection signalR { get; set; }
        public static Microsoft.AspNet.SignalR.Client.IHubProxy signalRhub { get; set; }

        public async void StartSignalRAsync()//khởi chạy kết nôi
        {
            int numberRetry2 = 0;
        RetryInvoice2:
            try
            {
             
                //https://stackoverflow.com/questions/11140164/signalr-console-app-example

                var url = "http://fnb.sposviet.vn/Signal";
                List<TimeSpan> timeSpans = new List<TimeSpan>(0);
                timeSpans.Add(TimeSpan.FromSeconds(1));
                timeSpans.Add(TimeSpan.FromSeconds(3));
                timeSpans.Add(TimeSpan.FromSeconds(5));
                timeSpans.Add(TimeSpan.FromSeconds(8));
                timeSpans.Add(TimeSpan.FromSeconds(10));
                timeSpans.Add(TimeSpan.FromSeconds(15));
                timeSpans.Add(TimeSpan.FromSeconds(20));
                timeSpans.Add(TimeSpan.FromSeconds(25));
                timeSpans.Add(TimeSpan.FromSeconds(30));
                timeSpans.Add(TimeSpan.FromSeconds(60));
                timeSpans.Add(TimeSpan.FromSeconds(70));
                timeSpans.Add(TimeSpan.FromSeconds(90));
                timeSpans.Add(TimeSpan.FromSeconds(120));
                timeSpans.Add(TimeSpan.FromSeconds(150));
                timeSpans.Add(TimeSpan.FromSeconds(170));
                timeSpans.Add(TimeSpan.FromSeconds(190));
                timeSpans.Add(TimeSpan.FromSeconds(200));
                timeSpans.Add(TimeSpan.FromSeconds(300));

                var connection = new HubConnectionBuilder()
                     .WithUrl(url)
                     .WithAutomaticReconnect(timeSpans.ToArray())
                     .Build();
                connection.On<string>("PrintbaobepSposViet", (html) => OnReceiveMessage(html));
                var t = connection.StartAsync();
                t.Wait();
                
                await connection.InvokeAsync("PrintbaobepSposViet", Properties.Settings.Default.ComId, "TEST");
                LogControl.Write("PrintbaobepSposViet test thành công");
            }
            catch (Exception e)
            {
                LogControl.Write(e.ToString());
                if (numberRetry2 < 20)
                {
                    numberRetry2++;
                    Thread.Sleep(4000);
                    goto RetryInvoice2;
                }
            }
        }
       
        private void OnReceiveMessage(string json)
        {
            try
            {
                var dataModel = JsonConvert.DeserializeObject<PrintModel>(json);
                if (dataModel?.type==Enum.EnumTypePrint.PrintBaoBep)
                {
                    PrintServer.PrintPageBaoBep(dataModel.data);
                }
                else
                {
                    LogControl.Write("Test ok: "+dataModel.data);
                }
            }
            catch (Exception e)
            {
                LogControl.Write(e.ToString());
            }
            
        }
    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Say(string message)
        {
            Trace.WriteLine("hub: " + message);
            Clients.All.AddMessage(message);
        }

        public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }


    public class ChatHub : Hub
    {
        //private static ConcurrentDictionary<string, User> ChatClients =
        //                                       new ConcurrentDictionary<string, User>();

        public override Task OnDisconnected(bool stopCalled)
        {
           
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        public void addMessage(string message)
        {
            Clients.All.addMessage(message);
        }
        public void PrintEinvoice(string message)
        {
            Clients.All.PrintEinvoice(message);
        }

        public void BroadcastTextMessage(string message)
        {
            var name = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(message))
            {
                Clients.Others.BroadcastTextMessage(name, message);
            }
        }

        public void BroadcastImageMessage(byte[] img)
        {
            var name = Clients.CallerState.UserName;
            if (img != null)
            {
                Clients.Others.BroadcastPictureMessage(name, img);
            }
        }
        //https://www.codeproject.com/Articles/1181555/SignalChat-WPF-SignalR-Chat-Application
        //public void UnicastTextMessage(string recepient, string message)
        //{
        //    var sender = Clients.CallerState.UserName;
        //    if (!string.IsNullOrEmpty(sender) && recepient != sender &&
        //        !string.IsNullOrEmpty(message) && ChatClients.ContainsKey(recepient))
        //    {
        //        User client = new User();
        //        ChatClients.TryGetValue(recepient, out client);
        //        Clients.Client(client.ID).UnicastTextMessage(sender, message);
        //    }
        //}

        //public void UnicastImageMessage(string recepient, byte[] img)
        //{
        //    var sender = Clients.CallerState.UserName;
        //    if (!string.IsNullOrEmpty(sender) && recepient != sender &&
        //        img != null && ChatClients.ContainsKey(recepient))
        //    {
        //        User client = new User();
        //        ChatClients.TryGetValue(recepient, out client);
        //        Clients.Client(client.ID).UnicastPictureMessage(sender, img);
        //    }
        //}
        //public void Typing(string recepient)
        //{
        //    if (string.IsNullOrEmpty(recepient)) return;
        //    var sender = Clients.CallerState.UserName;
        //    User client = new User();
        //    ChatClients.TryGetValue(recepient, out client);
        //    Clients.Client(client.ID).ParticipantTyping(sender);
        //}
    }
}
