using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcf_service
{
    public class LoggerClass
    {
        public StreamWriter file;
        private string filename;
        public LoggerClass()
        {
            filename = DateTime.Now.ToString("yyyy-MM-dd H.mm.ss");
            file = new StreamWriter($"{filename}.txt", append: true);
        }
        ~LoggerClass()
        {
            CloseLog();
        }
        public void WriteLog(string text)
        {
            file.WriteLine(text);
        }
        public void CloseLog()
        {
            file.Close();
        }
    }
}
