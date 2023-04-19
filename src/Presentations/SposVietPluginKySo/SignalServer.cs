using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Security.Policy;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.VisualBasic.ApplicationServices;
using Owin;

namespace SposVietPluginKySo
{
    public class SignalServer
    {//https://stackoverflow.com/questions/64551450/how-to-make-a-signalr-chat-in-winforms
        public static void Start()
        {
            var url = "https://localhost:8323/";
            //using (WebApp.Start<Startup>(new StartOptions(url: url)
            //{
            //    Port = 8323,
            //    ServerFactory = "Microsoft.Owin.Host.HttpListener"
            //})) {
            //    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //    context.Clients.All.addMessage("Server", "Hello from server");
            //}
            using (WebApp.Start<Startup>(url))
            {
                // Send message to all connected clients
                var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                context.Clients.All.addMessage("Server", "Hello from server");
                //  Console.WriteLine($"Server running at {url}");
                // Console.ReadLine();
            }
        }

    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR("/SposVietPlugin", new HubConfiguration());
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
