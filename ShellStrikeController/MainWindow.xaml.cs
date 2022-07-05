using ShellStrike;
using ShellStrike.WCFService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShellStrikeController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread InformationFetchingThread;
        ChannelFactory<IShellData> Channel;
        IShellData WCFIShellData { get; set; }
        List<ExecutableNode> ExecutableNodes { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ExecutableNodes = new List<ExecutableNode>();
            this.Closing += (x, y) =>
            {
                Channel.Abort();
                Process.GetCurrentProcess().Kill();
            };

        }

        private void RowButton_Click(object sender, RoutedEventArgs e)
        {

            ExecutableNode exeNode = (ExecutableNode)((Button)e.Source).DataContext;
            ExecutionWindow executionWindow = new ExecutionWindow(exeNode);
            executionWindow.Show();
        }

        private void gridClear_Click(object sender, RoutedEventArgs e)
        {
            {
                ExecutableNodes.Clear();
                gridExecList.Items.Clear();
            };
            GC.Collect();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                FillGrid();
        }

        void FillGrid()
        {
            gridExecList.Items.Clear();
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                ExecutableNodes.ForEach(X =>
                {
                    gridExecList.Items.Add(X);
                });
            }
            else
            {
                ExecutableNodes.FindAll(X => X.CIName.Contains(txtSearch.Text)).ForEach(X =>
                {
                    gridExecList.Items.Add(X);
                });
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Channel = new ChannelFactory<IShellData>(new NetTcpBinding(), new EndpointAddress(Properties.Settings.Default.ShellTCPBinding));
                Logger.Log("Channel Connected");
                WCFIShellData = Channel.CreateChannel(new EndpointAddress(Properties.Settings.Default.ShellTCPBinding));
            }
            catch (Exception t) { Logger.Error(t); }
            InformationFetchingThread = new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    try
                    {

                        if (Channel.State == CommunicationState.Faulted)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                Logger.Error("Channel Faulted");
                                lblStatus.Content = "Status: Faulted";
                                try
                                {
                                    Channel = new ChannelFactory<IShellData>(new NetTcpBinding(), new EndpointAddress(Properties.Settings.Default.ShellTCPBinding));
                                    WCFIShellData = Channel.CreateChannel(new EndpointAddress(Properties.Settings.Default.ShellTCPBinding));
                                    Logger.Log("Channel Connected");
                                    lblStatus.Content = "Status: Connected";
                                }
                                catch (Exception t) { Logger.Error(t); }
                            }));
                        }
                        DateTime ServiceStartupTime = WCFIShellData.GetServiceStartUpTime().Result;
                        int ActiveThreads = WCFIShellData.GetActiveThreadCount().Result;
                        int QueuedThreads = WCFIShellData.GetThreadsInQueueCount().Result;
                        Dispatcher.Invoke(new Action(delegate
                        {
                            lblServiceStartTime.Content = $"SS Time: {ServiceStartupTime}";
                            lblThreadStatus.Content = $"Active:{ActiveThreads}  Queued:{QueuedThreads}";
                            lblStatus.Content = "Status: Connected";
                        }));
                        var paths = WCFIShellData.GetRecentExecutionsPaths().Result;
                        //foreach (var path in paths)
                        //{
                        //    var node = XML.DeserializeObject<ExecutableNode>(System.IO.File.ReadAllText(path));
                        //    Dispatcher.Invoke(new Action(delegate
                        //    {
                        //        ExecutableNodes.Add(node);
                        //        FillGrid();
                        //    }));
                        //}
                    }
                    catch (Exception t)
                    {
                        Logger.Error(t);
                        Dispatcher.Invoke(new Action(delegate
                        {
                            lblThreadStatus.Content = "Status: Disconnected " + i.ToString();
                        }));
                    }
                    Thread.Sleep(2000);
                    i++;
                }

            });
            InformationFetchingThread.Start();
            btnConnect.IsEnabled = false;
        }

        private void btnServiceSettings_Click(object sender, RoutedEventArgs e)
        {
            ServiceSettingsWindow ssW = new ServiceSettingsWindow(WCFIShellData);
            ssW.Show();
        }

        private void btnExecuteNode_Click(object sender, RoutedEventArgs e)
        {
            NodeListWindow nodeListWindow = new NodeListWindow(WCFIShellData);
            nodeListWindow.Show();
        }


    }


}
