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
        string filePath = Environment.CurrentDirectory;

        public void DoLog(string message)
        {
            StreamWriter textFile = new StreamWriter(filePath);
            textFile.WriteLine(message);
        }
    }

    public interface ILogger
    {
        void DoLog(string message);
    }
}
