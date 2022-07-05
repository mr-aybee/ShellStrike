using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShellStrike
{
    public static class NetworkVals
    {

        public async static Task<bool> isConnectedWithPort(string IP, int Port)
        {
            return await Task.Run(() =>
            {
                try
                {
                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Connect(IP, Port);
                    if (tcpClient.Connected)
                        return true;
                }
                catch { return false; }
                return true;
            });
        }

        public async static Task<IPStatus> PingStatus(string ip, int timeout)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Ping ping = new Ping();
                    PingReply pr = ping.Send(ip, timeout);
                    return pr.Status;
                }
                catch { return IPStatus.NoResources; }
            });
        }



    }
}
