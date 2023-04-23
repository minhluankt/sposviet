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
using Owin;

namespace SposVietPluginKySo
{
    public class SignalServer
    {//https://stackoverflow.com/questions/64551450/how-to-make-a-signalr-chat-in-winforms
     //https://stackoverflow.com/questions/11140164/signalr-console-app-example
     //public static void Start()
     //{
     //    var url = "https://localhost:8323/";
     //    //using (WebApp.Start<Startup>(new StartOptions(url: url)
     //    //{
     //    //    Port = 8323,
     //    //    ServerFactory = "Microsoft.Owin.Host.HttpListener"
     //    //})) {
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
        static Microsoft.AspNet.SignalR.Client.HubConnection signalR { get; set; }
        public static Microsoft.AspNet.SignalR.Client.IHubProxy signalRhub { get; set; }

        public async void StartSignalRAsync()
        {
            try
            {
                //https://stackoverflow.com/questions/11140164/signalr-console-app-example
                var url = "https://localhost:7269/Signal";
                // signalR = new Microsoft.AspNet.SignalR.Client.HubConnection(url);
                //// signalR.Received += signalR_Received;
                // //signalRhub = signalR.CreateHubProxy("echo");
                // signalR.Start().Wait();
                // signalRhub.Invoke("Say", "hub invoked");

                var connection = new HubConnectionBuilder()
                     .WithUrl(url)
                     .WithAutomaticReconnect()
                     .Build();
                connection.On<string>("Printbaobep", (html) => OnReceiveMessage(html));

                var t = connection.StartAsync();

                t.Wait();

                // send a message to the hub
                await connection.InvokeAsync("Printbaobep", "ConsoleApp", "TEST");
            }
            catch (Exception e)
            {
            }
          
        }
        private void OnReceiveMessage(string html)
        {
            PrintServer.PrintPageBaoBep("OKPKPPPPPPPPPPP");
        }
    }
    //public class Startup
    //{
    //    public void Configuration(IAppBuilder app)
    //    {
    //        app.UseCors(CorsOptions.AllowAll);
    //        var url = "https://localhost:8323";
    //        app.MapSignalR(url+"/SposVietPrint", new HubConfiguration() { EnableJSONP = true });
    //    }
    //}
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
