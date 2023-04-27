using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Forms;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Net.WebSockets;
using WebSocketSharp.Server;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
namespace SposVietPluginKySo
{
    internal static class Program
    {

        public static string sSecret = "TextToEncrypt";
        static String hashAlgorithm;
        static String encryptionAlgorithm;
        static X509Certificate2 X509Certificate2;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        //static void Main()
        //{
        //    Application.Run(new LoginForm());
        //}

        static void Main()
        {
            bool ownmutex;
            // Tạo và lấy quyền sở hữu một Mutex có tên là SposVietPlugin;
            using (Mutex mutex = new Mutex(true, "SposVietPlugin", out ownmutex))
            {
                // Nếu ứng dụng sở hữu Mutex, nó có thể tiếp tục thực thi;
                // nếu không, ứng dụng sẽ thoát.
                if (ownmutex)
                {
                    StartInWindow();//khởi động cùng window

                    WebSocketSharpSposViet.StartWebSocket();//khởi tạo web socket
                    var signalRConnection = new SignalServer();
                    //signalRConnection.Start();
                    signalRConnection.StartSignalRAsync();

                    ApplicationConfiguration.Initialize();
                    Application.Run(new sposvietform());
                    //giai phong Mutex;
                    mutex.ReleaseMutex();
                }
                else
                    MessageBox.Show("Ứng dụng đang hoạt động", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }

        private static void StartInWindow()
        {
            //RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\SPOSVIET-PLUGIN");
            //mo registry khoi dong cung win
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
           // RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
          //  string keyvalue = "1";
            try
            {
                //chen gia tri key
                //regkey.SetValue("Index", keyvalue);
                //registryKey.SetValue("SposVietKySo", "");
                registryKey.SetValue("SPOSVIET_PLUGIN",$"\"{Application.StartupPath}SposVietPluginKySo.exe\"");
                //registryKey.SetValue("SPOSVIET-PLUGIN", Application.StartupPath + "SposVietPluginKySo.exe");
                
            }
            catch (System.Exception ex)
            {
            }
        }

        //public int ConnectToWebSocket()
        //{
        //   var ws = new WebSocket("ws://localhost:4434/?id=" + txtName.Text);
        //    ws.OnOpen += new EventHandler(ws_OnOpen);
        //    ws.OnMessage += new EventHandler<MessageEventArgs>(ws_OnMessage);
        //    ws.OnError += new EventHandler<ErrorEventArgs>(ws_OnError);
        //    ws.OnClose += new EventHandler<CloseEventArgs>(ws_OnClose);
        //    ws.Connect();

        //    var  ws = new WebSocket("ws://localhost:1337");

        //   var timer = new System.Windows.Forms.Timer();
        //    timer.Interval = 1500;
        //    timer.Tick += (sender, e) =>
        //    {
        //        if (ws != null)
        //            ws.Send(JsonConvert.SerializeObject(new { type = "getframe" }));
        //    };

        //    ws.OnMessage += Ws_OnMessage;
        //    ws.OnOpen += (sender, e) =>
        //    {
        //        //ws.Send(JsonConvert.SerializeObject(new { type = "getinfo" }));
        //        string msg = "{\"type\":\"getinfo\"}";
        //        ws.Send(msg);
        //    };
        //    ws.OnError += (sender, e) =>
        //    {
        //        MessageBox.Show("Error during Websocket Connection: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        ws.Close();
        //        ws = null;
        //        this.Dispose();
        //    };

        //    ws.Connect();
        //    if (Connected != null)
        //        Connected();
        //    timer.Start();

        //    return 0;
        //}

        static void CallWebSocket()
        {

            var wsClient = new ClientWebSocket();


        }
        private static Task OnError(ErrorEventArgs errorEventArgs)
        {
            Console.Write("Error: {0}, Exception: {1}", errorEventArgs.Message, errorEventArgs.Exception);
            return Task.FromResult(0);
        }

        private static Task OnMessage(MessageEventArgs messageEventArgs)
        {
            //Console.Write("Message received: {0}", messageEventArgs.Text.ReadToEnd());
            return Task.FromResult(0);
        }
        private static string EncryptRsa(string input)
        {
            string output = string.Empty;
            X509Certificate2 cert;
            if (X509Certificate2 != null)
            {
                cert = X509Certificate2;
            }
            else
            {
                cert = GetCertFromStore();
            }
           // X509Certificate2 cert = getCertificate(certificateName);

            // Changed from 
            //      using (RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key)
            // to
            //      using (System.Security.Cryptography.RSACng csp = (System.Security.Cryptography.RSACng)cert.PublicKey.Key)
            // because of 'Unable to cast object of type 'System.Security.Cryptography.RSACng' to type 'System.Security.Cryptography.RSACryptoServiceProvider'.'

            // Changed from 
            //      byte[] bytesEncrypted = csp.Encrypt(bytesData, false);
            // To            
            //      byte[] bytesEncrypted = csp.Encrypt(bytesData, System.Security.Cryptography.RSAEncryptionPadding.OaepSHA512);

            using (System.Security.Cryptography.RSACng csp = (System.Security.Cryptography.RSACng)cert.PublicKey.Key)
            {
                byte[] bytesData = Encoding.UTF8.GetBytes(input);
                //byte[] bytesEncrypted = csp.Encrypt(bytesData, false);
                byte[] bytesEncrypted = csp.Encrypt(bytesData, System.Security.Cryptography.RSAEncryptionPadding.Pkcs1);
                output = Convert.ToBase64String(bytesEncrypted);
            }
            return output;
        }

        private static string decryptRsa(string encrypted)
        {
            string text = string.Empty;
            X509Certificate2 cert;
            if (X509Certificate2 != null)
            {
                cert = X509Certificate2;
            }
            else
            {
                cert = GetCertFromStore();
            }
            // X509Certificate2 cert = getCertificate(certificateName);

            using (RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PrivateKey)
            //using (System.Security.Cryptography.RSACng csp = (System.Security.Cryptography.RSACng)cert.PrivateKey)
            {
                byte[] bytesEncrypted = Convert.FromBase64String(encrypted);
                byte[] bytesDecrypted = csp.Decrypt(bytesEncrypted, System.Security.Cryptography.RSAEncryptionPadding.Pkcs1);
                text = Encoding.UTF8.GetString(bytesDecrypted);
            }
            return text;
        }
        private static bool VerifyData(byte[] signature, string messageFromAhemd)
        {
            var messageHash = GetDataHash(messageFromAhemd);

            X509Certificate2? certificate = null;
            if (X509Certificate2 != null)
            {
                certificate = X509Certificate2;
            }
            else
            {
                certificate = GetCertFromStore();
            }
            using (System.Security.Cryptography.RSACng csp = (System.Security.Cryptography.RSACng)certificate.PublicKey.Key)
            {
                return csp.VerifyHash(messageHash, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
              //  RSACryptoServiceProvider cryptoServiceProvider = (RSACryptoServiceProvider)certificate.PublicKey.Key;

          //  return cryptoServiceProvider.VerifyHash(messageHash, CryptoConfig.MapNameToOID("SHA1"), signature);
        }

        private static byte[] GetDigitalSignature(byte[] hashBytes)
        {
           // X509Certificate2 certificate = GetCertFromStore();
            X509Certificate2? certificate = null;
            if (X509Certificate2!=null)
            {
                certificate = X509Certificate2;
            }
            else
            {
                certificate = GetCertFromStore();
            }
          
            /*use any asymmetric crypto service provider for encryption of hash   
            with private key of cert.   
            */
            RSACryptoServiceProvider rsaCryptoService = (RSACryptoServiceProvider)certificate.PrivateKey;

            /*now lets sign the hash   
            1.spevify hash bytes   
            2. and hash algorithm name to obtain the bytes   
            */
            return rsaCryptoService.SignHash(hashBytes, CryptoConfig.MapNameToOID("SHA1"));
           // return rsaCryptoService.SignHash(hashBytes);
        }

        private static byte[] GetDataHash(string sampleData)
        {
            //choose any hash algorithm    
            SHA1Managed managedHash = new SHA1Managed();

            return managedHash.ComputeHash(Encoding.Unicode.GetBytes(sampleData));
        }

        private static X509Certificate2 GetCertFromStore()
        {
            //to access to store we need to specify store name and location    
            X509Store x509Store = new X509Store(StoreLocation.CurrentUser);
           // X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            //store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            //obtain read only access to get cert    
            x509Store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            //  X509Certificate2Collection collection = (X509Certificate2Collection)x509Store.Certificates;
            // X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            // X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Chứng thư số", "Lựa chọn chứng thư số", X509SelectionFlag.MultiSelection);
            // Console.WriteLine("Number of certificates: {0}{1}", scollection.Count, Environment.NewLine);

            //foreach (X509Certificate2 x509 in scollection)
            //{
            //    try
            //    {
            //        byte[] rawdata = x509.RawData;
            //        Console.WriteLine("Content Type: {0}{1}", X509Certificate2.GetCertContentType(rawdata), Environment.NewLine);
            //        Console.WriteLine("Friendly Name: {0}{1}", x509.FriendlyName, Environment.NewLine);
            //        Console.WriteLine("Certificate Verified?: {0}{1}", x509.Verify(), Environment.NewLine);
            //        Console.WriteLine("Simple Name: {0}{1}", x509.GetNameInfo(X509NameType.SimpleName, true), Environment.NewLine);
            //        Console.WriteLine("Signature Algorithm: {0}{1}", x509.SignatureAlgorithm.FriendlyName, Environment.NewLine);
            //        Console.WriteLine("Public Key: {0}{1}", x509.PublicKey.Key.ToXmlString(false), Environment.NewLine);
            //        Console.WriteLine("Certificate Archived?: {0}{1}", x509.Archived, Environment.NewLine);
            //        Console.WriteLine("Length of Raw Data: {0}{1}", x509.RawData.Length, Environment.NewLine);
            //        X509Certificate2UI.DisplayCertificate(x509);
            //        x509.Reset();
            //    }
            //    catch (CryptographicException)
            //    {
            //        Console.WriteLine("Information could not be written out for this certificate.");
            //    }
            //}
            // Display a dialog box to select a certificate from the Windows Store
            X509Certificate2Collection selectedCertificates =
                    X509Certificate2UI.SelectFromCollection(x509Store.Certificates, "Chứng thư số", "Lựa chọn chứng thư số", X509SelectionFlag.SingleSelection);

            // Get the first certificate that has a primary key
            foreach (var certificate in selectedCertificates)
            {
                if (certificate.HasPrivateKey)
                    X509Certificate2 = certificate;
                    return certificate;
            }
            return x509Store.Certificates[0];
        }
        public static string Sign(this X509Certificate2 x509, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] signedData;

            using (MD5 hasher = MD5.Create())
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PrivateKey;
                signedData = rsa.SignData(data, hasher);
            }
            return Convert.ToBase64String(signedData);// +Environment.NewLine + Environment.NewLine; 
                                                      //return ByteArrayToString(signedData); //Convert.ToBase64String(signedData);
        }
        //Console.WriteLine("WS server started on ws://localhost:7890/SposVietPlugin");
        //Console.WriteLine("WS server started on ws://localhost:7890/EchoAll");

        // Console.ReadKey();
        // wssv.Stop();
        ///end


        // X509Certificate2 = null;//taọ null khi chạy
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.



        //retrieve certificate from store//
        ////https://stackoverflow.com/questions/58342269/unable-to-cast-object-of-type-rsacng-to-type-system-security-cryptography-rsa




        //ví dụ để ký Encrypt
        // Encryption 
        //string sEncryptedSecret = string.Empty;
        //sEncryptedSecret = EncryptRsa(sSecret);

        //// Decryption 
        //string sDecryptedSecret = string.Empty;
        //sDecryptedSecret = decryptRsa(sEncryptedSecret);


        //**signing data**//    

        //to sign we need the hash of data//    
        // byte[] hashBytes = GetDataHash(messageToFatima);



        //X509Certificate2 certificate = GetCertFromStore();//gọi show token
        //string messageToFatima = "vBHl5jb/fXZQVt5mo539t0F2owg=";//hashtruyeefn vào
        //byte[] hashBytes = Convert.FromBase64String(messageToFatima);//chuyển hash truyền vào vì nó truyền bas464

        ////https://www.c-sharpcorner.com/article/visual-studio-creating-and-managing-digital-certificates-in-c-sharp/
        //byte[] signature = GetDigitalSignature(hashBytes);//ký số
        //var sSecret = Convert.ToBase64String(signature);//chuyển về base64
        //XmlDocument xmlDoc = new XmlDocument();
        //XmlNode nVersion = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        //xmlDoc.AppendChild(nVersion);
        //XmlNode DLieu = xmlDoc.CreateElement("SendInv");
        //xmlDoc.AppendChild(DLieu);

        //XmlNode Base64Hash = xmlDoc.CreateElement("Base64Hash");
        //Base64Hash.InnerText= messageToFatima;
        //DLieu.AppendChild(Base64Hash);

        //XmlNode SignValue = xmlDoc.CreateElement("SignValue");
        //SignValue.InnerText = sSecret;
        //DLieu.AppendChild(SignValue);
        //var xml = xmlDoc.InnerXml;//gửi xml
        //bool isVerified = VerifyData(signature, messageToFatima);
        //đến đây thôi nhế


    }
}