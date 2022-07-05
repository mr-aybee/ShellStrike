using ShellStrike.WCFService;
using ShellStrike.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellStrike
{
    public static class ServiceCache
    {
        public static string PLinkApplicationPath { get; set; }

        public static bool DoRawOutputLogs { get; set; } = false;
        public static bool DoOutputLogs { get; set; } = false;
        public static bool DoProcessLogs { get; set; } = false;


        //Startup Properties
        public static DateTime StartupTime { get; set; }

        //
        public static int ThreadLimit { get; set; } = 20;
        public static int ThreadMaxTimeout { get; set; } = 60000;
        public static int FetchInterval { get; set; } = 60000;
        public static int OutputRetryAttemps { get; set; } = 5;
        public static int CommandWait { get; set; } = 2000;
        public static int CommandEndTimeout { get; set; } = 15000;
        /// 

        public static bool FetchNext { get; set; } = true;

        public static int NodesInExecution { get; set; } = 0;
        public static int NodesInQueue { get; set; } = 0;

        public static Queue<string> QExecutedPaths { get; set; }

        public static Queue<ExecutableNode> LocalExecutions { get; set; }

        public static List<ExecutionInfo> LastExecutions { get; set; }

        public static DateTime LastProcessExitCheckup { get; set; }
    }

}
