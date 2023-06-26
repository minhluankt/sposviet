using Microsoft.Win32;
using SposVietPlugin_net_4._6._1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SposVietPlugin_net_4._6._1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}
        static   void Main()
        {
            LogControl.Write("Bắt đầu chạy");
            bool ownmutex;
            // Tạo và lấy quyền sở hữu một Mutex có tên là SposVietPlugin;
            using (Mutex mutex = new Mutex(true, "SposVietPluginPrint", out ownmutex))
            {
                // Nếu ứng dụng sở hữu Mutex, nó có thể tiếp tục thực thi;
                // nếu không, ứng dụng sẽ thoát.
                if (ownmutex)
                {
                    Thread.Sleep(800);
                    StartInWindow();//khởi động cùng window

                    WebSocketSharpSposViet.StartWebSocket();//khởi tạo web socket
                    var signalRConnection = new SignalServer();
                    //signalRConnection.Start();
                     signalRConnection.StartSignalRAsync().Wait();
                    mutex.ReleaseMutex();
                    LogControl.Write("Ứng dụng đã chạy");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new sposvietform());
                    //giai phong Mutex;

                }
                else
                {
                    LogControl.Write("Vui lòng kiểm tra lại ứng dụng, có thể đang hoạt động!");
                    MessageBox.Show("Vui lòng kiểm tra lại ứng dụng, có thể đang hoạt động!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static void StartInWindow()
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\SPOSVIET_PLUGIN");
            //mo registry khoi dong cung win
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            // RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";
            try
            {
                //chen gia tri key
                regkey.SetValue("Index", keyvalue);
                //registryKey.SetValue("SposVietKySo", "");
                registryKey.SetValue("SPOSVIET_PLUGIN", $"\"{Application.StartupPath}\\SposVietPlugin4.6.1.exe\"");
                //registryKey.SetValue("SPOSVIET-PLUGIN", Application.StartupPath + "SposVietPlugin_net_4._6._1.exe");

            }
            catch (System.Exception ex)
            {
            }
        }

    }

}
