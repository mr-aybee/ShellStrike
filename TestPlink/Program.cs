using ShellStrike;
using ShellStrike.Card;
using ShellStrike.WCFService;
using ShellStrike.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace TestPlink
{
    class Program
    {

        static void Main(string[] args)
        {
            {
                ExecutableNode executableNode = new ExecutableNode
                {
                    CIName = "A",
                    IP = "192.168.8.112",
                    Arguments = "-ssh 192.168.8.112",
                    AccountName = "root",
                    ExecutionCode = "1234_1234_ABC",

                };
                executableNode.SetEncryptedCardData(File.ReadAllText(@"C:\Users\malik\OneDrive\Desktop\LoginCard.xml"));
                PlinkHandler plinkHandler = new PlinkHandler(executableNode);
                plinkHandler.StartProc();
                Console.Title = plinkHandler._PlinkProcess.ProcessName;
                ExecCard execCard = new ExecCard(executableNode, ref plinkHandler);
                execCard.Start();
                var b = executableNode;

                var a = b.AsXml();
                //
                File.WriteAllText(b.ExecutionCode + ".xml", a);
                Console.Write(a);
                a = string.Empty;
                plinkHandler.ExitAndClear();
                plinkHandler = null;
                execCard.Clear();
                execCard = null;
            };
            GC.Collect();
            Console.ReadKey();


            //ServiceReference1.ItestWCFClient itestWCFClient = new ServiceReference1.ItestWCFClient();
            ////Console.Write(itestWCFClient.DoWork("Test"));
            //var a = await itestWCFClient.ExecuteHSAsync("");
            //Console.Write(a);
            //Console.ReadKey();

            //NetTcpBinding binding = new NetTcpBinding();
            //EndpointAddress addr = new EndpointAddress("net.tcp://localhost:5000/testWCF");
            //ChannelFactory<ItestWCF> chn = new ChannelFactory<ItestWCF>(binding, addr);
            //idd = chn.CreateChannel();
            //if (chn.State != CommunicationState.Opened) { }
            //var a = await idd.ExecuteHS("");
            //chn.Close();
            //Console.WriteLine(a);
            //Console.ReadKey();
            //return 1;
        }
    }
}
