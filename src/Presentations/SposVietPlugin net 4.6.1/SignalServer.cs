using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SposVietPlugin_net_4._6._1.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SposVietPlugin_net_4._6._1
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

        public async Task<bool> StartSignalRAsync()//khởi chạy kết nôi
        {
            bool connected = false;
        //    int numberRetry2 = 0;
        //RetryInvoice2:
            try
            {

                //https://stackoverflow.com/questions/11140164/signalr-console-app-example
                string domain = !string.IsNullOrEmpty(Properties.Settings.Default.Domain) ? Properties.Settings.Default.Domain : "https://fnb.sposviet.vn";
                if (!domain.Contains("https"))
                {
                    domain = "https://fnb.sposviet.vn";
                }
                //domain = "https://localhost:7269";
                var url = $"{domain}/Signal";
                List<TimeSpan> timeSpans = new List<TimeSpan>();
                for (int i = 0; i < 100; i++)
                {
                    timeSpans.Add(TimeSpan.FromSeconds(1));
                }
               
               
                var connection = new HubConnectionBuilder()
                    .WithUrl(url)
                    .WithAutomaticReconnect(timeSpans.ToArray())
                    .Build();
                connection.ServerTimeout = TimeSpan.FromHours(15);

                connection.On<string>("PrintbaobepSposViet", (html) => OnReceiveMessage(html));
                await connection.StartAsync();
                // t.Wait();
                if (Properties.Settings.Default.ComId > 0)
                {
                    await connection.InvokeAsync("PrintbaobepSposViet", Properties.Settings.Default.ComId, "TEST");
                    LogControl.Write("PrintbaobepSposViet test thành công");
                }
                else
                {
                    LogControl.Write("Comid faile:" + Properties.Settings.Default.ComId);
                }
                if (connection.State == HubConnectionState.Connected)
                {
                    connection.Closed += (exception) => {
                        LogControl.Write("exception Closed:" + exception.ToString());
                        Connection_StateChangedCore(HubConnectionState.Connected);
                        return Task.CompletedTask;
                    };
                    connected = true;
                }





                //var connection = new HubConnection(url);

                //var _hubProxy = connection.CreateHubProxy("SposVietPluginTool");
                //connection.Start().ContinueWith(task => {
                //    if (task.IsFaulted)
                //     {
                //         string err = task.Exception.GetBaseException().ToString();
                //         LogControl.Write(task.Exception.GetBaseException().ToString());
                //    }
                //    else
                //    {
                //        Console.WriteLine("Connected");
                //    }

                //}).Wait();

                //_hubProxy.On<string>("PrintbaobepSposViet",
                //      (html) => OnReceiveMessage(html));
                //if (Properties.Settings.Default.ComId > 0)
                //{
                //    await _hubProxy.Invoke("PrintbaobepSposViet", Properties.Settings.Default.ComId, "TEST");
                //    LogControl.Write("PrintbaobepSposViet test thành công");
                //}
                //else
                //{
                //    LogControl.Write("Comid faile:" + Properties.Settings.Default.ComId);
                //}


                //if (connection.State == ConnectionState.Connected)
                //{
                //    connection.Error += (ex) =>
                //    {
                //        LogControl.Write("Connection error: " + ex.ToString());
                //    };
                //    connection.Closed += () =>
                //    {
                //        LogControl.Write("Connection closed");
                //    };
                //    connection.StateChanged += Connection_StateChanged;
                //    Console.WriteLine("Server for Current is started.");
                //    connected = true;
                //}



                return connected;

            }
            catch (Exception e)
            {
                LogControl.Write("RestartConnection: " + e.ToString());
                await RestartConnection();
                return connected;
            }
        }
        private async void Connection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Disconnected)
            {
                await RestartConnection();
            }
        } 
        private async void Connection_StateChangedCore(HubConnectionState obj)
        {
            //if (obj == HubConnectionState.Disconnected)
            {
                await RestartConnection();
            }
        }
        public async Task RestartConnection()
        {
            bool ApplicationClosed =false;
            while (!ApplicationClosed)
            {
                bool connected = await StartSignalRAsync();
                if (connected)
                    return;
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
                    LogControl.Write("In bill OK");
                }
                else
                {
                    LogControl.Write("Test ok: "+dataModel?.data);
                }
            }
            catch (Exception e)
            {
                LogControl.Write(e.ToString());
            }
            
        }
    }

}
