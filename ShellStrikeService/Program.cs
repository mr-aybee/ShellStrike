using ShellStrike;
using ShellStrike.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;

namespace ShellStrikeService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            StartupChecks();
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new MainService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception t) { Logger.Error(t); }
        }


        static void StartupChecks()
        {
            try
            {
                Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                ServiceCache.StartupTime = DateTime.Now;
                ServiceCache.FetchNext = true;
                ServiceCache.ThreadLimit = Properties.Settings.Default.ThreadLimit;
                ServiceCache.ThreadMaxTimeout = Properties.Settings.Default.ThreadMaxTimeout;
                ServiceCache.FetchInterval = Properties.Settings.Default.FetchInterval;
                ServiceCache.CommandEndTimeout = Properties.Settings.Default.CommandEndTimeout;
                ServiceCache.CommandWait = Properties.Settings.Default.CommandWait;
                ServiceCache.OutputRetryAttemps = Properties.Settings.Default.OutputRetryAttemps;
                ServiceCache.DoOutputLogs = Properties.Settings.Default.DoOutputLogs;
                ServiceCache.DoRawOutputLogs = Properties.Settings.Default.DoRawOutputLogs;
                ServiceCache.DoProcessLogs = Properties.Settings.Default.DoProcessLogs;
                DataBaseOps.dBColdStrikeConnectionString = Properties.Settings.Default.dBColdStrikeConnectionString;
                ServiceCache.QExecutedPaths = new Queue<string>();
                ServiceCache.PLinkApplicationPath = Properties.Settings.Default.PlinkPath;
            }
            catch (Exception t) { Logger.Error(t); }
        }
    }
}
