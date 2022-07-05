using ShellStrike.Card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ShellStrike.Shell
{
    public class PlinkHandler
    {
        public string Name { get; set; }
        public static string pLinkPath = "C:\\Program Files\\PuTTY\\plink.exe";

        public Process _PlinkProcess { get; set; }

        public bool Connected { get; set; } = false;

        public PlinkHandler(string arguments, string Name)
        {
            _PlinkProcess = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = !string.IsNullOrEmpty(ServiceCache.PLinkApplicationPath) ?
                                ServiceCache.PLinkApplicationPath : pLinkPath,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            _PlinkProcess.StartInfo = processStartInfo;
            _PlinkProcess.EnableRaisingEvents = true;

            _PlinkProcess.Exited += (x, y) =>
            {
                Connected = false;
                Thread.Sleep(2000);
            };

            Process.GetCurrentProcess().Exited += (x, y) =>
            {
                Connected = false;
                try { _PlinkProcess.Kill(); } catch { }
            };
        }

        ExecutableNode ExecutableNode { get; set; }

        public PlinkHandler(ExecutableNode executableNode)
        {
            ExecutableNode = executableNode;
            _PlinkProcess = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                Arguments = executableNode.Arguments,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = !string.IsNullOrEmpty(ServiceCache.PLinkApplicationPath) ?
                                ServiceCache.PLinkApplicationPath : pLinkPath
            };
            _PlinkProcess.StartInfo = processStartInfo;
            _PlinkProcess.EnableRaisingEvents = true;

            _PlinkProcess.Exited += (x, y) =>
            {
                Connected = false;
            };

            Process.GetCurrentProcess().Exited += (x, y) =>
            {
                Connected = false;
                try { _PlinkProcess.Kill(); } catch { }
            };


        }

        public void StartProc()
        {
            _PlinkProcess.Start();
            Connected = true;
            Thread.Sleep(500);
        }

        public void ExitAndClear()
        {
            Connected = false;
            Thread.Sleep(2000);
            try
            {
                Process.GetProcessById(_PlinkProcess.Id).Kill();
            }
            catch { }
            try { _PlinkProcess.Dispose(); } catch { }
            //GC.Collect();
        }


        public void SendCommand(string Command)
        {
            _PlinkProcess.StandardInput.Write(Command);
            _PlinkProcess.StandardInput.Flush();
            //{
            //    if (_PlinkProcess.StandardInput.BaseStream.CanWrite)
            //    {
            //        //byte[] bytesOfCC = _PlinkProcess.StandardInput.Encoding.GetBytes(Command);
            //        //byte[] bytesOfCC = _PlinkProcess.StandardInput.Encoding.GetBytes(Command);
            //        //_PlinkProcess.StandardInput.BaseStream.Write(bytesOfCC, 0, bytesOfCC.Length);
            //        //bytesOfCC = null;
            //    }
            //    _PlinkProcess.StandardInput.BaseStream.Flush();
            //};
        }

        public async Task SendCommandAsync(string Command)
            => await Task.Run(() => { SendCommand(Command); });

        public void HandleRegistryKey()
        {
            if (_PlinkProcess.StandardError.BaseStream.CanRead == false) return;
            SendCommand("");
            string[] BreakCases = new string[] {
                "(y/n",
                "thumbprint",
                "server's host key is not cached in the registry",
                "adding the key to the cache, enter"
            };

            if (File.Exists("ThumbprintCases.txt")) BreakCases = File.ReadAllLines("ThumbprintCases.txt");
            StringBuilder stringBuilder = new StringBuilder();
            var outputText = Output(_PlinkProcess.StandardError);
            foreach (char c in outputText.ToCharArray())
            {
                stringBuilder.Append(c);
                if (stringBuilder.EndsWith(BreakCases))
                {
                    SendCommand("y" + "\n");
                    Logger.Log($"{ExecutableNode.ExecutionCode}[]RegistryError[]{stringBuilder}[]y\n");
                    break;
                }
            }
            try
            {
                _PlinkProcess.StandardError.DiscardBufferedData();
                _PlinkProcess.StandardError.BaseStream.Flush();
            }
            catch (Exception t)
            {
                Logger.Log($"{ExecutableNode.ExecutionCode} Flushing StandardError Stream Givves Error []{t.Message}");
            }
            outputText = "";
            stringBuilder.Clear();
        }

        public BreakCaseReturn ReadBrokenOutput(List<BreakCase> BreakCases, int commandEndTimeout = 500)
        {
            //if (commandEndTimeout == 0) commandEndTimeout = 15000;

            BreakCaseReturn breakCaseReturn = new BreakCaseReturn();
            if (_PlinkProcess.StandardOutput.BaseStream.CanRead == false)
            {
                Logger.Log("StandardOutput Cannot Read");
                return breakCaseReturn;
            }

            BufferedStream bs = new BufferedStream(_PlinkProcess.StandardOutput.BaseStream);
            byte[] b = new byte[1];

            StringBuilder stringBuilder = new StringBuilder();
            BreakCase breakCase = null;
            Thread.Sleep(15);
            FieldInfo fieldReadLength = typeof(BufferedStream).GetField("_readLen", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldReadPosition = typeof(BufferedStream).GetField("_readPos", BindingFlags.NonPublic | BindingFlags.Instance);

            int readLength = 10;
            int readPos = 0;
            while (readPos < readLength)
            {
                bs.Read(b, 0, 1);
                stringBuilder.Append(UTF8Encoding.UTF8.GetString(b));
                readPos = (int)fieldReadPosition.GetValue(bs);
                readLength = (int)fieldReadLength.GetValue(bs);
                breakCaseReturn.Output = stringBuilder?.ToString();
                if (stringBuilder.EndsWith(BreakCases, out breakCase))
                {
                    breakCaseReturn.Output = stringBuilder?.ToString();
                    breakCaseReturn.CaseReturn = breakCase;

                    bs.Flush();
                    break;
                }
            }

            try
            {
                _PlinkProcess.StandardOutput.DiscardBufferedData();
                _PlinkProcess.StandardOutput.BaseStream.Flush();
            }
            catch (Exception t)
            {
                Logger.Error($"{ExecutableNode.ExecutionCode} Flushing StandardOutPut Stream Gives Error []{t.Message}");
                Logger.Log($"{ExecutableNode.ExecutionCode} Flushing StandardOutPut Stream Gives Error []{t.Message}");
            }
            /// Read From Error
            return breakCaseReturn;
        }

        string Output(StreamReader outputStream)
        {
            StringBuilder stringBuilder = new StringBuilder();
            char[] b = new char[1];
            int tries = 0;
            bool isStreamReading = false;
            while (isStreamReading == false && tries < 3)
            {
                Thread T = new Thread(() => { outputStream.Read(b, 0, 1); });
                T.Priority = ThreadPriority.Lowest;
                T.Start();
                isStreamReading = T.Join(5000);
                stringBuilder.Append(b);
                tries++;
            }
            if (!isStreamReading) return stringBuilder.ToString();
            int readLen = (int)typeof(StreamReader).GetField("charLen", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(outputStream);
            if (readLen == 0) return stringBuilder.ToString();

            int readPos = 1;
            while (readPos < readLen)
            {
                outputStream.Read(b, 0, 1);
                stringBuilder.Append(b);
                readPos++;
            }
            outputStream.DiscardBufferedData();
            return stringBuilder.ToString();
        }

        public BreakCaseReturn ReadBrokenOutputFromPlain(List<BreakCase> BreakCases, int commandEndTimeout = 2000)
        {
            if (commandEndTimeout == 0) commandEndTimeout = 2000;
            StringBuilder stringBuilder = new StringBuilder();
            BreakCaseReturn breakCaseReturn = new BreakCaseReturn();
            BreakCase breakCase = new BreakCase();
            var outputText = Output(_PlinkProcess.StandardOutput);
            foreach (char c in outputText.ToCharArray())
            {
                stringBuilder.Append(c);
                if (stringBuilder.EndsWith(BreakCases, out breakCase))
                {
                    breakCaseReturn.Output = stringBuilder?.ToString();
                    breakCaseReturn.CaseReturn = breakCase;
                    break;
                }
            }
            return breakCaseReturn;
        }
    }

    public class BreakCaseReturn
    {
        public string Output { get; set; }
        public BreakCase CaseReturn { get; set; }
    }

}
