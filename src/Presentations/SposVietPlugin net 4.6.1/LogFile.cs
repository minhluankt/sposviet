using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SposVietPlugin_net_4._6._1
{
    public class LogControl
    {
        private static string _Path = string.Empty;
        private static bool DEBUG = true;

        public static void Write(string msg)
        {
            _Path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            try
            {
                var checkp = Path.Combine(_Path,"Log");
                if (!Directory.Exists(checkp))
                {
                    Directory.CreateDirectory(checkp);
                }
                using (StreamWriter w = File.AppendText(Path.Combine(checkp, DateTime.Now.ToString("ddMMyyyy")+"log.txt")))
                {
                    Log(msg, w);
                }
               // if (DEBUG)
                //    Console.WriteLine(msg);
            }
            catch (Exception e)
            {
                //Handle
            }
        }

        static private void Log(string msg, TextWriter w)
        {
            try
            {
                
                w.Write("[{0} {1}]", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                w.Write("\t");
                w.WriteLine(" {0}", msg);
                w.WriteLine("-----------------------");
            }
            catch (Exception e)
            {
                //Handle
            }
        }
    }
}
