using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SposVietPluginKySo
{
    public class ClientWebSocketForm
    {
        ClientWebSocket wsClient = new ClientWebSocket();
        Encoding encoding = UTF8Encoding.UTF8;
        string _url = "wss://localhost:45325";
        public async Task OpenConnectionAsync(CancellationToken token)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpListener _httpListener = new HttpListener();
                //SerialPort ports = new SerialPort("4433", 9600, Parity.None, 8, StopBits.One);
                //ports.Open();
                _httpListener.Prefixes.Add("https://localhost:45325/"); // add prefix "http://localhost:5000/"
                _httpListener.Start(); // start server (Run application as Administrator!)
                //In case you need proxy
                //  wsClient.Options.Proxy = Proxy;
                //Set keep alive interval
                wsClient.Options.KeepAliveInterval = TimeSpan.Zero;

                //Set desired headers
                // wsClient.Options.SetRequestHeader("Host", "4434");

                //Add sub protocol if it's needed
                // wsClient.Options.AddSubProtocol("zap-protocol-v1");

                //Add options if compression is needed
                wsClient.Options.DangerousDeflateOptions = new WebSocketDeflateOptions
                {
                    ServerContextTakeover = true,
                    ClientMaxWindowBits = 15
                };

                await wsClient.ConnectAsync(new Uri(_url), CancellationToken.None);
                var rcvTask = Task.Run(Receive);
                // await wsClient.ConnectAsync(new Uri(_url), token).ConfigureAwait(false);
                await SendAsync("OK");
                while (wsClient.State == WebSocketState.Open)
                {
                    var msg = await ReadString(wsClient);

                    //do something with your message...
                }
            }
            catch (Exception e)
            {

               
            }
          
        }
         void Receive()
        {
            var rcvBuff = new byte[1 << 10];
            while (true)
            {
                var rcvMessage = wsClient.ReceiveAsync(rcvBuff, CancellationToken.None).Result;
                if (rcvMessage.MessageType == WebSocketMessageType.Text)
                {
                    var rcvText = encoding.GetString(rcvBuff, 0, rcvMessage.Count);
                    Console.WriteLine($"Received message: \"{rcvText}\"");
                }
            }
        }
        //Send message
        public async Task SendAsync(string message)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            await wsClient.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
        }
        public  async Task<String> ReadString(ClientWebSocket socket)
        {
            var reciveBuffer = new byte[32000];

            var result = await socket.ReceiveAsync(new ArraySegment<byte>(reciveBuffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

            return Encoding.ASCII.GetString(reciveBuffer, 0, result.Count);
        }//Receiving messages
        private async Task ReceiveMessageAsync(byte[] buffer)
        {
            while (true)
            {
                try
                {
                    var result = await wsClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);

                    //Here is the received message as string
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (result.EndOfMessage) break;
                }
                catch (Exception ex)
                {
                   // _logger.LogError("Error in receiving messages: {err}", ex.Message);
                    break;
                }
            }
        }

        public async Task HandleMessagesAsync(CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            while (wsClient.State == WebSocketState.Open)
            {
                await ReceiveMessageAsync(buffer);
            }
            if (wsClient.State != WebSocketState.Open)
            {
                //_logger.LogInformation("Connection closed. Status: {s}", WsClient.State.ToString());
                // Your logic if state is different than `WebSocketState.Open`
            }
        }
        //Re
    }
}
