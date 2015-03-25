using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FilePost.Util
{
    public class Logger
    {
        public static string FileName = "log.txt";
        public static string Path = "data";

        private  FileStream fileStream = null;

        private static Logger logger = null;

        public static Logger Instance
        {
            get
            {
                if (logger == null)
                {
                    logger = new Logger();
                }
                return logger;
            }
        }

        private Logger()
        {
            string fullname = System.IO.Path.Combine(Path, FileName);
            fileStream = new FileStream(fullname, FileMode.Append);
        }

        ~Logger()
        {
            fileStream.Close();
        }

        public void Print(string text)
        {
            StreamWriter sw= new StreamWriter(fileStream);
            sw.AutoFlush = true;
            sw.Write("[" + DateTime.Now.ToString() + "] " + text);
            sw.Close();
        }
    }
}
