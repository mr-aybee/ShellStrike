using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ShellStrike.WCFService
{
    public class ShellData : IShellData
    {

        public string DoWork(string A)
        {
            return $"Message is {A}";
        }

        public async Task<int> SetCacheValueOf(string Parameter, string value, bool SetPermanent)
        {
            return await Task.Run(() =>
            {
                try
                {
                    switch (Parameter)
                    {

                        case "ThreadLimit":
                            ServiceCache.ThreadLimit = value.ToInt32();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "ThreadMaxTimeout":
                            ServiceCache.ThreadMaxTimeout = value.ToInt32();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "FetchInterval":
                            ServiceCache.FetchInterval = value.ToInt32();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "CommandWait":
                            ServiceCache.CommandWait = value.ToInt32();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "CommandEndTimeout":
                            ServiceCache.CommandEndTimeout = value.ToInt32();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "FetchNext":
                            ServiceCache.FetchNext = value.ToBoolean();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "DoOutputLogs":
                            ServiceCache.DoOutputLogs = value.ToBoolean();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "DoRawOutputLogs":
                            ServiceCache.DoRawOutputLogs = value.ToBoolean();
                            if (SetPermanent)
                            {

                            }
                            break;
                        case "DoProcessLogs":
                            ServiceCache.DoProcessLogs = value.ToBoolean();
                            if (SetPermanent)
                            {

                            }
                            break;
                    }
                    return 1;
                }
                catch { return -1; }
            });
        }

        public async Task<List<Pair>> GetCacheValues()
        {
            return await Task.Run(() =>
            {
                return new List<Pair> {
                    new Pair("FetchNext",ServiceCache.FetchNext),
                    new Pair("ThreadLimit",ServiceCache.ThreadLimit),
                    new Pair("ThreadMaxTimeout",ServiceCache.ThreadMaxTimeout),
                    new Pair("FetchInterval",ServiceCache.FetchInterval),
                    new Pair("CommandWait",ServiceCache.CommandWait),
                    new Pair("CommandEndTimeout",ServiceCache.CommandEndTimeout),
                    new Pair("ThreadLimit",ServiceCache.ThreadLimit),
                    new Pair("DoOutputLogs",ServiceCache.DoOutputLogs),
                    new Pair("DoRawOutputLogs",ServiceCache.DoRawOutputLogs),
                    new Pair("DoProcessLogs",ServiceCache.DoProcessLogs),
                };
            });
        }


        public void SetMaxThreadLimit(int Count)
        {
            ServiceCache.ThreadLimit = Count;
        }

        public async Task<int> GetActiveThreadCount()
        {
            return await Task.Run(() => { return ServiceCache.NodesInExecution; });
        }

        public async Task<int> GetThreadsInQueueCount()
        {
            return await Task.Run(() => { return ServiceCache.NodesInQueue; });
        }

        public async Task<DateTime> GetServiceStartUpTime()
        {
            return await Task.Run(() => { return ServiceCache.StartupTime; });
        }

        public async Task<List<ExecutionInfo>> GetRecentExecutionsPaths()
        {

            return await Task.Run(() =>
            {
                object obg = new object();
                List<ExecutionInfo> execs = new List<ExecutionInfo>();
                lock (obg)
                {
                    execs = ServiceCache.LastExecutions;
                    ServiceCache.LastExecutions.Clear();
                };
                return execs;
            });
        }


        public Task<int> ExecuteNode(string CIName, string AccountName)
        {
            return Task.Run(() =>
            {
                if (ServiceCache.LocalExecutions == null)
                    ServiceCache.LocalExecutions = new Queue<ExecutableNode>();
                string Query = @"
SELECT ID,CIName,IP,22 Port,Arguments,AccountName,'SHELL-LOGINCHECKS' ExecutionType from shell_ConnectivityMonitor sCM 
	CROSS APPLY (
		SELECT TOP(1) IP FROM sso_ConfigurationItems CI with(nolock) WHERe CI.CIName = sCM.CINAme
	) A
	CROSS APPLY (
		SELECT TOP(1) Arguments FROM Batches B with(nolock) WHERe B.BatchName = sCM.CINAme
	) B
WHERE CIName = @CIName ANd AccountName = @AccountName ANd ServerName = dbo.getClientHostName()
";
                var dT = DataBaseOps.GetDataSet(Query, new Hashtable {
                    {"CIName",CIName },
                    { "AccountName",AccountName}
                }).Tables[0];
                foreach (DataRow dR in dT.Rows)
                    ServiceCache.LocalExecutions.Enqueue(new ExecutableNode(dR));
                return 1;
            });
        }

        public async Task<DataSet> GetDSFromDB(string Query, string Pairs)
        {
            return await Task.Run(() =>
            {
                Hashtable hashtable = new Hashtable();
                if (!string.IsNullOrEmpty(Pairs))
                {
                    List<Pair> parsedPairs = new List<Pair>();
                    parsedPairs = XML.DeserializeObject<List<Pair>>(Pairs);
                    foreach (Pair p in parsedPairs)
                        hashtable.Add(p.Name, p.Value?.ToString());
                }
                return DataBaseOps.GetDataSet(Query, hashtable);
            });
        }
    }
}
