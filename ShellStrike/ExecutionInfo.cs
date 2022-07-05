namespace ShellStrike
{
    public class ExecutionInfo
    {
        public string ExecutionCode { get; set; }
        public string CIName { get; set; }
        public string AccountName { get; set; }
        public string ExecutionTime { get; set; }
        public string ExecutionType { get; set; }
        public string DetailsPath { get; set; }

        public ExecutionInfo(string executionCode, string cIName, string accountName, string executionTime, string executionType, string detailsPath)
        {
            ExecutionCode = executionCode;
            CIName = cIName;
            AccountName = accountName;
            ExecutionTime = executionTime;
            ExecutionType = executionType;
            DetailsPath = detailsPath;
        }
    }

}
