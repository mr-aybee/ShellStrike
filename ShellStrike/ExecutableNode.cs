using ShellStrike.Card;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ShellStrike
{
    [Serializable]
    public class ExecutableNode
    {

        public long ID { get; set; }
        public string ExecutionCode { get; set; }
        public string ExecuteWith { get; set; }
        public string ExecutionType { get; set; }
        public string Arguments { get; set; }
        public string CIName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string AccountName { get; set; }
        public DateTime StartTime { get; set; }
        public bool isCompleted { get; set; }
        public DateTime EndTime { get; set; }
        public string Error { get; set; }


        public string CardData { get; set; }

        public List<CommandLog> CommandLogs { get; set; }

        public IPStatus PingReply { get; set; }
        public bool TCPTelnetStatus { get; set; }

        public ExecutableNode() { }

        public ExecutableNode(DataRow dR)
        {
            ID = dR["ID"].ToInt64();
            ExecutionType = dR["ExecutionType"]?.ToString();
            this.CIName = dR["CIName"]?.ToString();
            this.Arguments = dR["Arguments"]?.ToString();
            this.IP = dR["IP"]?.ToString();
            this.AccountName = dR["AccountName"]?.ToString();
            this.ExecutionCode = $"{DateTime.Now.ToString("yyyyMMdd hhmm")}_{ScalarFunctions.GetRandomString(5)}_{CIName}";
            this.Port = dR["Port"].ToInt32();
            this.ExecuteWith = dR["ExecuteWith"]?.ToString();
            
            if (Port == 0) Port = 22;

            CommandLogs = new List<CommandLog>();
        }


        public string GetDecryptedCardData()
        => Encoding.UTF8.GetString(Convert.FromBase64String(CardData));

        public void SetEncryptedCardData(string PlainCard)
        => CardData = Convert.ToBase64String(Encoding.UTF8.GetBytes(PlainCard));

    }

}
