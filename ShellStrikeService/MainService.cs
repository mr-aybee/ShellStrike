using ShellStrike;
using ShellStrike.WCFService;
using ShellStrike.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShellStrikeService
{
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
            this.ServiceName = "XC-ShellStrikeSrv";
        }

        ShellService shellService;
        ServiceHost srv;
        protected override void OnStart(string[] args)
        {
            try
            {
                srv = new ServiceHost(typeof(ShellData));
                srv.AddServiceEndpoint(typeof(IShellData),
                    new NetTcpBinding(),
                    Properties.Settings.Default.ShellTCPBinding);
                srv.Open();
                Logger.Log($"Host Listening at {Properties.Settings.Default.ShellTCPBinding}");
                //
                shellService = new ShellService();
                shellService.SetupThreads();
                shellService.Start();
                ///
            }
            catch (Exception t) { Logger.Error(t); }
        }

        protected override void OnStop()
        {
            shellService.Stop();
            srv.Abort();
            Process.GetCurrentProcess().Kill();
        }


    }
}
