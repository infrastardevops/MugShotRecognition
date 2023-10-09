using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MugShotApp
{
    internal class Logger
    {
        string LoggerFile = @"C:\ProgramData\MugShotApp\debug.log";
        string OuputFile = @"C:\ProgramData\MugShotApp\output.txt";

        public void Log(string entry)
        {
            Console.WriteLine(entry);
            File.AppendAllText(LoggerFile, "\n ["+ DateTime.Now.ToString() + "]: " + entry);
        }

        public void LogOuput(string entry)
        {
            Console.WriteLine(entry);
            File.AppendAllText(OuputFile, "\n [" + DateTime.Now.ToString() + "]: " + entry);
        }
    }
}
