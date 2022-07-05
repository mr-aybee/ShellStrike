using Renci.SshNet;
using ShellStrike.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShellStrike.Shell
{
    public class ShellService
    {

        Thread NodeFetcherThread { get; set; }
        Thread InformationBuilderThread { get; set; }
        ThreadPooler ThreadPooler { get; set; }

        public ShellService()
        {
            ServiceCache.LocalExecutions = new Queue<ExecutableNode>();
        }

        public void SetupThreads()
        {
            ThreadPooler = new ThreadPooler(ServiceCache.ThreadLimit);
            NodeFetcherThread = new Thread(() =>
            {
                while (true)
                {
                    if (ServiceCache.FetchNext)
                    {
                        {
                            try
                            {
                                //
                                var dT = DataBaseOps.GetDataSet("EXEC shell_getNodesForExecution").Tables[0];
                                foreach (DataRow dR in dT.Rows)
                                {
                                    ThreadPooler.AddToQueue(new ExecutableNode(dR));
                                }
                                Logger.Log($"Enqueued {dT.Rows.Count} more nodes");
                                dT.Clear();
                                dT.Dispose();
                            }
                            catch (Exception t) { Logger.Error(t); }
                        };
                        GC.Collect();
                        //
                    }
                    Thread.Sleep(ServiceCache.FetchInterval);
                    Logger.Log($"Fetch After Interval {ServiceCache.FetchInterval}");
                }
            });

            InformationBuilderThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    ServiceCache.NodesInExecution = ThreadPooler.ActiveThreadCount;
                    ServiceCache.NodesInQueue = ThreadPooler.QueueCount;
                    ThreadPooler.MaxThreadLimit = ServiceCache.ThreadLimit;

                    // For Execution by Attached Controller manually
                    if (ServiceCache.LocalExecutions.Count > 0)
                    {
                        for (int i = 0; i < ServiceCache.LocalExecutions.Count; i++)
                        {
                            ThreadPooler.AddToQueue(ServiceCache.LocalExecutions.Dequeue());
                        }
                    }
                    try
                    {
                        if (ThreadPooler.ActiveThreadCount == 0)
                        {
                            //  Handle Not Closed Child Processes of Plink
                            if ((DateTime.Now - ServiceCache.LastProcessExitCheckup).TotalMinutes > 5)
                            {
                                ServiceCache.LastProcessExitCheckup = DateTime.Now;
                                Process.GetCurrentProcess().GetChildProcesses().ToList().ForEach(X =>
                                {
                                    if (!X.HasExited) X.Kill();

                                });
                            }
                        }
                    }
                    catch (Exception t) { Logger.Error(t); }
                }
            });
        }



        public void Start()
        {

            NodeFetcherThread.Start();
            Logger.Log($"NodeFetcher Started");
            ThreadPooler.StartExecution((obj) =>
            {
                ExecuteNode((ExecutableNode)obj);
            });
            Logger.Log($"ThreadPooler Execution Started");
            InformationBuilderThread.Start();
            Logger.Log($"InformationBuilder Started");
        }

        public void Stop()
        {
            NodeFetcherThread.Abort();
            ThreadPooler.StartExecution((obj) =>
            {
                ExecuteNode((ExecutableNode)obj);
            });
            while (ThreadPooler.ActiveThreadCount != 0) { Thread.Sleep(300); }
            InformationBuilderThread.Abort();
            ThreadPooler.Abort();
        }



        void ExecuteNode(ExecutableNode node)
        {
            try
            {
                Logger.Log($"{node.ExecutionCode}[]Executing Node");

                var sFlow = ExecuteShellFlow(node);

                var pathForExecutions = $"Executions\\Executions {DateTime.Now.ToString("yyyyMMdd")}";
                Directory.CreateDirectory(pathForExecutions);
                var filePath = pathForExecutions + "\\" + sFlow.ExecutionCode + ".xml";
                var fileContent = (sFlow).AsXml();
                File.WriteAllText(filePath, fileContent);
                ServiceCache.QExecutedPaths.Enqueue(new FileInfo(filePath).FullName);
                filePath = string.Empty;
                fileContent = string.Empty;

            }
            catch (Exception t) { Logger.Error(t); }
            GC.Collect();
        }

        ExecutableNode ExecuteShellFlow(ExecutableNode exeNode)
        {

            // Ping // Check Telnet
            exeNode.PingReply = NetworkVals.PingStatus(exeNode.IP, 5000).Result;
            exeNode.TCPTelnetStatus = NetworkVals.isConnectedWithPort(exeNode.IP, exeNode.Port).Result;
            var updateDT = DataBaseOps.GetDataSetAsync($"UPDATE [shell_ConnectivityMonitor] SET Ping = '{exeNode.PingReply}',Telnet = '{exeNode.TCPTelnetStatus}', MonitorTime = GETDATE() WHERE ID = {exeNode.ID}", new Hashtable()).Result;
            Logger.Log($"{exeNode.ExecutionCode}[]Ping:{exeNode.PingReply}[]Telnet:{exeNode.TCPTelnetStatus}");            // Card For Connectivity and Login

            if (exeNode.ExecuteWith == "RENCI")
            {
                Logger.Log($"{exeNode.ExecutionCode}[]{exeNode.AccountName}[]RENCI");
                exeNode = ExecuteRenci(exeNode);
            }
            else if (exeNode.ExecuteWith == "PUTTY")
            {
                Logger.Log($"{exeNode.ExecutionCode}[]{exeNode.AccountName}[]PUTTY");
                exeNode = ExecutePutty(exeNode);
            }
            Logger.ExecutionEntry($"{exeNode.ExecutionCode}\t{exeNode.ExecutionType}\t{exeNode.CIName}\t{exeNode.IP}\t{exeNode.Port}\t{exeNode.AccountName}\t{exeNode.Arguments}");
            //
            GC.Collect();
            return exeNode;
        }


        ExecutableNode ExecuteRenci(ExecutableNode exeNode)
        {
            Thread.Sleep(500);
            try
            {
                var renciDSTemplateEditor = DataBaseOps.GetDataSet($"EXEC [dbo].[shell_TemplateEditor] @CIName ='{exeNode.CIName}', @AccountName='{exeNode.AccountName}',@ExecutionType='{exeNode.ExecutionType}', @ExecuteWith='RENCI'");
                var LoginDT = renciDSTemplateEditor.Tables[0];
                var CommandFlowDT = renciDSTemplateEditor.Tables[1];
                var password = LoginDT.Rows[0]["Password"]?.ToString();
                var keyboardAuthMethod = new KeyboardInteractiveAuthenticationMethod(exeNode.AccountName);
                keyboardAuthMethod.AuthenticationPrompt += (x, y) =>
                {
                    foreach (var prompt in y.Prompts)
                    {
                        if (prompt.Request.ToLowerInvariant().StartsWith("password") || prompt.Request.ToLowerInvariant().Contains("ssword"))
                            prompt.Response = password;
                        if (prompt.Request.ToLowerInvariant().Contains("login as") || prompt.Request.ToLowerInvariant().Contains("username") || prompt.Request.ToLowerInvariant().Contains("login:"))
                            prompt.Response = exeNode.AccountName;
                        Logger.Log($"{exeNode.ExecutionCode}[]{exeNode.AccountName}[]RENCI[]KeyboardAuthentication[Req]{prompt.Request}[Res]{prompt.Response}");
                    }
                };
                ConnectionInfo connectionInfo = new ConnectionInfo(
                    exeNode.IP,
                    exeNode.AccountName,
                    new PasswordAuthenticationMethod(exeNode.AccountName, password),
                    keyboardAuthMethod
                   );
                using (var SshClient = new SshClient(connectionInfo))
                {
                    try
                    {
                        SshClient.Connect();
                    }
                    catch (Exception t)
                    {
                        Logger.Error(t);
                        if (t.Message.ToLower().Contains("permission denied (password)") || t.Message.ToLower().Contains("password") || t.Message == "Permission denied (keyboard-interactive)")
                            DataBaseOps.GetDataSet($"UPDATE shell_ConnectivityMonitor SET Connectivity=1,Login=0 ,ErrorMessage = @Error WHERE ID = {exeNode.ID} AND ServerName = dbo.getClientHostName()", new Hashtable { { "Error", t.Message } });
                        else
                            DataBaseOps.GetDataSet($"UPDATE shell_ConnectivityMonitor SET ErrorMessage = @Error WHERE ID = {exeNode.ID} AND ServerName = dbo.getClientHostName()", new Hashtable { { "Error", t.Message } });
                        exeNode.Error = t.Message;
                        return exeNode;
                    }
                    DataBaseOps.GetDataSet($"UPDATE shell_ConnectivityMonitor SET Login = 1 , Connectivity = 1 WHERE ID = {exeNode.ID} AND ServerName = dbo.getClientHostName()");

                    exeNode.CommandLogs = new List<CommandLog>();
                    foreach (DataRow commandDataRow in CommandFlowDT.Rows)
                    {
                        Thread.Sleep(50);
                        try
                        {
                            string responseText = "";
                            string commandText = "";
                            commandText = commandDataRow["Command"]?.ToString();
                            SshCommand execCommmand = SshClient.CreateCommand(commandText);
                            exeNode.CommandLogs.Add(new CommandLog(CommandDirection.INPUT, CommandType.Renci, commandText, "", exeNode.ExecutionCode));
                            responseText = execCommmand.Execute();
                            Logger.Log($"{exeNode.ExecutionCode}[]{exeNode.AccountName}[]Input[]{commandText}");
                            if (ServiceCache.DoOutputLogs || ServiceCache.DoRawOutputLogs)
                                Logger.Log($"{exeNode.ExecutionCode}[]{exeNode.AccountName}[]Input[]{responseText}");
                            exeNode.CommandLogs.Add(new CommandLog(CommandDirection.OUTPUT, CommandType.Renci, "FULL", responseText, exeNode.ExecutionCode));
                            // Execte AfterSQL
                            DataBaseOps.GetDataSet(commandDataRow["AfterSQL"]?.ToString(), new Hashtable
                                {
                                    { "LastOutput",responseText},
                                    { "CIName",exeNode.CIName},
                                    { "AccountName",exeNode.AccountName}
                                });
                        }
                        catch (Exception t)
                        {
                            DataBaseOps.GetDataSet($"UPDATE shell_ConnectivityMonitor SET ErrorMessage = @Error WHERE ID = {exeNode.ID} AND ServerName = dbo.getClientHostName()", new Hashtable { { "Error", t.Message } });
                            exeNode.Error = t.Message;
                            Logger.Error(t);
                            break;
                        }
                    }
                    connectionInfo = null;
                }
                renciDSTemplateEditor.Dispose();
            }
            catch (Exception t) { Logger.Error(t); exeNode.Error = t.Message; }
            GC.Collect();
            return exeNode;
        }


        ExecutableNode ExecutePutty(ExecutableNode exeNode)
        {
            PlinkHandler plinkHandler = new PlinkHandler(exeNode);
            plinkHandler.StartProc();
            Logger.Log($"{exeNode.ExecutionCode}[]Process Started");
            string cardData = DataBaseOps.GetDataSet($"EXEC [dbo].[shell_TemplateEditor] @CIName ='{exeNode.CIName}', @AccountName='{exeNode.AccountName}',@ExecutionType='{exeNode.ExecutionType}', @ExecuteWith='PUTTY'")
                                .Tables[0].Rows[0][0]?.ToString();
            exeNode.SetEncryptedCardData(cardData);
            ExecCard execCard = new ExecCard(exeNode, ref plinkHandler);
            bool execCardReturn = false;
            Task.Run(() =>
            {
                execCardReturn = execCard.Start();
            }).Wait(ServiceCache.ThreadMaxTimeout);
            Logger.Log($"{exeNode.ExecutionCode}[]Card Return With {execCardReturn}");
            Thread.Sleep(500);
            plinkHandler.ExitAndClear();
            execCard.Clear();
            cardData = string.Empty;
            exeNode = execCard.ExecutableNode;
            return exeNode;
        }
    }
}
