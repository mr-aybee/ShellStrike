using ShellStrike.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;


namespace ShellStrike.Card
{

    public class ExecCard
    {
        PlinkHandler Plink { get; set; }
        public XmlDocument XmlDocument { get; set; }

        public string LastOutput { get; set; }
        public string EndWith { get; set; }

        public ExecutableNode ExecutableNode { get; set; }

        public Exception CardError { get; set; }

        public ExecCard(ExecutableNode executableNode, ref PlinkHandler plink)
        {
            ExecutableNode = executableNode;
            ExecutableNode.CommandLogs = new List<CommandLog>();
            XmlDocument = new XmlDocument();
            XmlDocument.LoadXml(executableNode.GetDecryptedCardData());
            Plink = plink;
        }

        public bool Start()
        {
            try
            {
                var sDT = DateTime.Now;
                ExecutableNode.StartTime = DateTime.Now;

                Logger.Log($"{ExecutableNode.ExecutionCode}[]Card Execution Started");
                ///Pre Check
                Plink.HandleRegistryKey();

                foreach (XmlNode C in XmlDocument.SelectSingleNode("Root").ChildNodes)
                {
                    if (C.Name == "Command")
                    {
                        Command commandNode = new Command(C);
                        Thread.Sleep(500);
                        bool CommandExecVal = ExecCommand(commandNode);
                        if (CommandExecVal == false) return false;
                        // For BreakWith With Exit successfully
                        if (EndWith == "true") return true;
                    }
                }

                Logger.Log($"{ExecutableNode.ExecutionCode}[]Card Execution Ended");
                ExecutableNode.EndTime = DateTime.Now;
                ExecutableNode.isCompleted = true;
                return true;
            }
            catch (Exception t) { Logger.Error(t); CardError = t; return false; }
        }

        public async Task<bool> StartAsync()
        {
            return await Task.Run(() =>
            {
                return Start();
            });
        }


        public void Clear()
        {
            XmlDocument = null;
            LastOutput = string.Empty;
            Logger.Log($"{ExecutableNode.ExecutionCode} Card Cleared");
        }



        bool ExecCommand(Command command)
        {
            try
            {
                List<BreakCase> breakCases = new List<BreakCase>();
                foreach (XmlNode bNode in command.ChildNodes)
                    if (bNode.Name == "BreakCase")
                        breakCases.Add(new BreakCase(bNode));

                Hashtable hT = new Hashtable();
                hT.Add("LastOutput", LastOutput);

                if (!string.IsNullOrEmpty(command.Condition))
                {
                    bool ConditionFromDB = DataBaseOps.GetDataSet(command.Text, hT).Tables[0].Rows[0][0].ToBoolean();
                    ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.INPUT, CommandType.SQLCondition, command.Text, ConditionFromDB.ToString(), ExecutableNode.ExecutionCode));
                    if (ConditionFromDB == false) return true;
                }

                if (command.CommandMode == "SQL")
                {
                    string CommandFromDB = DataBaseOps.GetDataSet(command.Text, hT).Tables[0].Rows[0][0]?.ToString();
                    ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.INPUT, CommandType.SQLCommand, command.Text, CommandFromDB, ExecutableNode.ExecutionCode));
                    Logger.Log($"{ExecutableNode.ExecutionCode}[][]Command SQL[]{command.Text}[][]Command Text[]{CommandFromDB}");
                    Plink.SendCommand(CommandFromDB);
                }
                else
                {
                    Logger.Log($"{ExecutableNode.ExecutionCode}[][]Command Text[]{command.Text}");
                    Plink.SendCommand(command.Text);
                    ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.INPUT, CommandType.CommandText, command.Text, "", ExecutableNode.ExecutionCode));
                }


                Thread.Sleep(ServiceCache.CommandWait);
                BreakCaseReturn breakCaseOut = new BreakCaseReturn();
                LastOutput = "<Waiting For Next Output>";
                //breakCaseOut = Plink.ReadBrokenOutput(breakCases, ServiceCache.CommandEndTimeout + command.CommandTimeout);
                breakCaseOut = Plink.ReadBrokenOutputFromPlain(breakCases, ServiceCache.CommandEndTimeout + command.CommandTimeout);
                LastOutput = breakCaseOut.Output;

                if (ServiceCache.DoRawOutputLogs)
                    Logger.Log($"{ ExecutableNode.ExecutionCode} []Raw[]{LastOutput}");

                if (breakCaseOut.CaseReturn == null)
                {
                    Logger.Error($"{ExecutableNode.ExecutionCode} No BreakCase Found");
                    Logger.Log($"{ExecutableNode.ExecutionCode} No BreakCase Found[LastOutput]{LastOutput}");
                    return false;
                }


                ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.OUTPUT, CommandType.BreakCase, breakCaseOut.CaseReturn.Text, LastOutput, ExecutableNode.ExecutionCode));
                if (ServiceCache.DoOutputLogs)
                    Logger.Log($"{ExecutableNode.ExecutionCode}[][]BREAK[]{breakCaseOut?.CaseReturn?.Text}[][]OUTPUT[]{LastOutput}");

                if (!string.IsNullOrEmpty(breakCaseOut.CaseReturn.EndWith))
                {
                    if (breakCaseOut.CaseReturn.EndWith.ToLower() == "true")
                    {
                        Logger.Log($"{ExecutableNode.ExecutionCode} ExitWith [True]");
                        ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.OUTPUT, CommandType.BreakCase, "CardExit", "ExitWith [True]", ExecutableNode.ExecutionCode));
                        return true;
                    }
                    if (breakCaseOut.CaseReturn.EndWith.ToLower() == "false")
                    {
                        Logger.Log($"{ExecutableNode.ExecutionCode} ExitWith [False]");
                        ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.OUTPUT, CommandType.BreakCase, "CardExit", "ExitWith [False]", ExecutableNode.ExecutionCode));

                        return false;
                    }
                    EndWith = breakCaseOut.CaseReturn.EndWith.ToLower();
                }

                // Go Inside BreakCases
                foreach (XmlNode Node in breakCaseOut.CaseReturn.ChildNodes)
                {
                    if (Node.Name == "Process")
                    {
                        bool ProcessExecVal = ExecProcess(new xProcess(Node));
                        if (!ProcessExecVal) return false;
                    }
                    if (Node.Name == "Command")
                    {
                        bool CommandExecVal = ExecCommand(new Command(Node));
                        if (!CommandExecVal) return false;
                    }
                }
                breakCaseOut = null;
                return true;
            }
            catch (Exception t)
            {
                Logger.Error(t); CardError = t;
                return false;
            }
        }


        bool ExecProcess(xProcess ProcessNode)
        {
            try
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("LastOutput", LastOutput);
                DataSet dS;
                if (!string.IsNullOrEmpty(ProcessNode.BeforeSQL))
                {

                    dS = DataBaseOps.GetDataSet(ProcessNode.BeforeSQL, hashtable);
                    if (ServiceCache.DoProcessLogs)
                        Logger.Log($"{ExecutableNode.ExecutionCode}[]Process:  {ProcessNode.BeforeSQL}");
                    ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.PROCESS, CommandType.ProcessBeforeSQL, ProcessNode.BeforeSQL, "", ExecutableNode.ExecutionCode));
                }
                if (!string.IsNullOrEmpty(ProcessNode.AfterSQL))
                {
                    dS = DataBaseOps.GetDataSet(ProcessNode.AfterSQL, hashtable);
                    if (ServiceCache.DoProcessLogs)
                        Logger.Log($"{ExecutableNode.ExecutionCode}[]Process:  {ProcessNode.AfterSQL}");
                    ExecutableNode.CommandLogs.Add(new CommandLog(CommandDirection.PROCESS, CommandType.ProcessAfterSQL, ProcessNode.AfterSQL, "", ExecutableNode.ExecutionCode));
                }
                dS = null;
                hashtable.Clear();
                if (ProcessNode.pNodes.Count > 0)
                {
                    // Go Inside Processes
                    foreach (XmlNode Node in ProcessNode.pNodes)
                    {
                        if (Node.Name == "Process")
                        {
                            bool ProcessExecVal = ExecProcess(new xProcess(Node));
                            if (!ProcessExecVal) return false;
                        }
                        if (Node.Name == "Command")
                        {
                            bool CommandExecVal = ExecCommand(new Command(Node));
                            if (!CommandExecVal) return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception t)
            {
                Logger.Error(t); CardError = t;
                return false;
            }

        }
    }

}
