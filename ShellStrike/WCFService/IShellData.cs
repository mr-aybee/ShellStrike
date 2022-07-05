using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ShellStrike.WCFService
{

    [ServiceContract]
    public interface IShellData
    {
        [OperationContract]
        string DoWork(string A);

        [OperationContract]
        Task<int> SetCacheValueOf(string Paramerter, string Value, bool SetPermanent);

        [OperationContract]
        Task<List<Pair>> GetCacheValues();

        [OperationContract]
        void SetMaxThreadLimit(int Count);

        [OperationContract]
        Task<DateTime> GetServiceStartUpTime();

        [OperationContract]
        Task<int> GetActiveThreadCount();

        [OperationContract]
        Task<int> GetThreadsInQueueCount();

        [OperationContract]
        Task<List<ExecutionInfo>> GetRecentExecutionsPaths();

        [OperationContract]
        Task<int> ExecuteNode(string CIName, string AccountName);

        [OperationContract]
        Task<DataSet> GetDSFromDB(string Query, string Pairs);
    }
}
