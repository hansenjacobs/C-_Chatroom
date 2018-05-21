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
        StreamWriter textFile;

        public TextLogger()
        {
            filePath = Environment.CurrentDirectory;
            textFile = new StreamWriter(filePath + "\\log.txt");
        }
        public void DoLog(string message)
        {
            textFile.WriteLine(message);
        }
    }

    public interface ILogger
    {
        void DoLog(string message);
    }
}
