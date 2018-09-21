using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFunction
{
    public class LogHelper
    {
        private static ILog log;

        private static ILog Logger {
            get {
                if (log == null) {
                    log = log4net.LogManager.GetLogger("Jacky.Logging");
                }
                return log;
            }
        }

        public static void InfoLog(string message) {
            //log4net.ILog log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器

            Logger.Info(DateTime.Now.ToString() + ": " + message);//写入一条新log
        }

        public static void ErrorLog(string message,Exception ex) {
            //log4net.ILog log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器

          

            log.Fatal(DateTime.Now.ToString() + ": " + message, ex);

            
        }

       
    }
}
