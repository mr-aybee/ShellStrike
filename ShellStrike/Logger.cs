using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellStrike
{

    public static class Logger
    {
        public static Logging Logging { get; set; } = new Logging();

        public static void Log(string Text)
        {
            Logging.WriteLogToFile($"{DateTime.Now}\t{Text}");
        }

        public static void Error(string Message)
        {
            Logging.WriteErrorToFile($"{DateTime.Now}\t{Message}");
        }

        public static void Error(Exception Ex)
        {
            Logging.WriteErrorToFile(
                $"{DateTime.Now}[]\r\n" +
                $"Message: {Ex.Message}\r\n" +
                $"StackTrace: {Ex.StackTrace}\r\n"
            );
        }

        public static void ExecutionEntry(string Text)
        {
            Logging.WriteExecutionEntryToFile($"{DateTime.Now}\t{Text}\n");
        }

    }


    public class Logging
    {
        public Logging() { }

        private static readonly object locker = new object();

        public void WriteLogToFile(string message)
        {
            lock (locker)
            {
                Directory.CreateDirectory("Logs");
                StreamWriter SW;
                SW = File.AppendText($"Logs\\Logs {DateTime.Now.ToString("yyyyMMdd")}.txt");
                SW.WriteLine(message);
                SW.Close();
            }
        }

        public void WriteExecutionEntryToFile(string message)
        {
            lock (locker)
            {
                StreamWriter SW;
                SW = File.AppendText($"Executions.txt");
                SW.WriteLine(message);
                SW.Close();
            }
        }

        public void WriteErrorToFile(string message)
        {
            lock (locker)
            {
                Directory.CreateDirectory("Errors");
                StreamWriter SW;
                SW = File.AppendText($"Errors\\ErrorLogs {DateTime.Now.ToString("yyyyMMdd")}.txt");
                SW.WriteLine(message);
                SW.Close();
            }
        }
    }
}
