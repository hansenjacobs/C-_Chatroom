using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    class TextLogger : ILogger
    {
        string filePath;
        public TextLogger()
        {
            filePath = Environment.CurrentDirectory;
        }
        public void DoLog(string message)
        {
            using (StreamWriter textFile = new StreamWriter(filePath + "\\log.txt", true))
            {
                textFile.WriteLine(message);
                textFile.Close();
            }
        }
    }

    public interface ILogger
    {
        void DoLog(string message);
    }
}
