using ShellStrike;
using ShellStrike.WCFService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
    /// Interaction logic for NodeListWindow.xaml
    /// </summary>
    public partial class NodeListWindow : Window
    {
        IShellData _ShellData { get; set; }
        public NodeListWindow(IShellData shellData)
        {
            InitializeComponent();
            _ShellData = shellData;
            //Loaded += (x, y) =>
            //{
            //    SetUpGrid();
            //};
        }

        void SetUpGrid()
        {
            try
            {
                var filterQuery = "";
                if (!string.IsNullOrEmpty(searchTxt.Text) || searchTxt != null)
                {
                    searchTxt.Text += ",";
                    var filterItems = searchTxt.Text.Split(',').Where(X => !string.IsNullOrEmpty(X)).ToArray();

                    if (filterItems.Length == 1)
                        filterQuery += $" AND CIName like '%{filterItems[0]}%' ";
                    if (filterItems.Length == 2)
                        filterQuery += $" AND AccountName like '%{filterItems[1]}%' ";
                }
                var dT = _ShellData.GetDSFromDB(" SELECT * from shell_ConnectivityMonitor with(nolock) WHERE ServerName = dbo.getClientHostName() " + filterQuery, null).Result.Tables[0];
                gridExecList.Items.Clear();
                foreach (DataRow item in dT.Rows)
                {
                    gridExecList.Items.Add(
                    new ConnectivityNodes(item["ID"]?.ToString(),
                        item["CIName"]?.ToString(),
                        item["AccountName"]?.ToString(),
                        item["Ping"]?.ToString(),
                        item["Telnet"]?.ToString(),
                        item["Connectivity"]?.ToString(),
                        item["Login"]?.ToString(),
                        item["MonitorTime"]?.ToString()));
                }
            }
            catch (Exception t)
            {
                Logger.Error(t);
                MessageBox.Show(t.Message);
            }
        }

        private void searchTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetUpGrid();
            }
        }

        private void btnRefeshAll_Click(object sender, RoutedEventArgs e)
        {

            foreach (DataGridRow I in gridExecList.SelectedItems)
            {
                var conn = (ConnectivityNodes)I.DataContext;
                MessageBox.Show(conn.ID);
            }
        }

        private void gridExecList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void RowButton_Click(object sender, RoutedEventArgs e)
        {
            var conn = (ConnectivityNodes)((Button)e.Source).DataContext;
            _ShellData.ExecuteNode(conn.CIName, conn.AccountName);
            MessageBox.Show("Node Executed");
        }
    }

    public class ConnectivityNodes
    {
        public string ID { get; set; }
        public string CIName { get; set; }
        public string AccountName { get; set; }
        public string Ping { get; set; }
        public string Telnet { get; set; }
        public string Connectivity { get; set; }
        public string Login { get; set; }
        public string MonitorTime { get; set; }

        public ConnectivityNodes(string iD, string cIName, string accountName, string ping, string telnet, string connectivity, string login, string monitorTime)
        {
            ID = iD;
            CIName = cIName;
            AccountName = accountName;
            Ping = ping;
            Telnet = telnet;
            Connectivity = connectivity;
            Login = login;
            MonitorTime = monitorTime;
        }
    }
}
