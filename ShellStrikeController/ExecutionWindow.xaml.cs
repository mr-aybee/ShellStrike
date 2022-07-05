using ShellStrike;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ExecutionWindow.xaml
    /// </summary>
    public partial class ExecutionWindow : Window
    {
        ExecutableNode ExecutableNode { get; set; }
        public ExecutionWindow()
        {
            InitializeComponent();
        }


        public ExecutionWindow(string xmlPath)
        {
            InitializeComponent();
            var cDaata = File.ReadAllText(xmlPath);
            var exeNode = XML.DeserializeObject<ExecutableNode>(cDaata);
            ExecutableNode = exeNode;
            SetUpExeNode(exeNode);
        }


        public ExecutionWindow(ExecutableNode ExeNode)
        {
            ExecutableNode = ExeNode;
            InitializeComponent();
            SetUpExeNode(ExeNode);
        }

        void SetUpExeNode(ExecutableNode ExeNode)
        {
            Title = ExeNode.ExecutionCode;
            executionCodeTxt.Text = ExeNode.ExecutionCode;
            ciNameTxt.Text = ExeNode.CIName;
            ipTxt.Text = ExeNode.IP;
            accountNameTxt.Text = ExeNode.AccountName;
            argumentsTxt.Text = ExeNode.Arguments;
            executionTypeTxt.Text = ExeNode.ExecutionType;
            portTxt.Text = ExeNode.Port.ToString();
            txtPing.Content = $"Ping {ExeNode.PingReply}";
            txtTelnet.Content = $"Telnet ({portTxt.Text}) {ExeNode.TCPTelnetStatus}";
            txtExecutionCompleted.Content = $"Execution Completed: {ExeNode.isCompleted} ";
            var exeRecord = ExeNode;
            if (exeRecord == null) return;
            var commandLogs = exeRecord.CommandLogs;
            if (commandLogs == null || commandLogs.Count == 0) return;

            foreach (var cL in commandLogs)
            {

                switch (cL.CommandDirection)
                {
                    case CommandDirection.INPUT:
                        chatConsolePanel.Children.Add(new Controls.MiniControls.ucInputMessage(cL));
                        break;
                    case CommandDirection.OUTPUT:
                        chatConsolePanel.Children.Add(new Controls.MiniControls.ucOutputBox(cL));
                        break;
                    case CommandDirection.PROCESS:
                        chatConsolePanel.Children.Add(new Controls.MiniControls.ucProcessBox(cL));
                        break;
                }
            }

        }

        private void btnCardUsed_Click(object sender, RoutedEventArgs e)
        {
            new CardViewWindow(ExecutableNode.GetDecryptedCardData()).Show() ;
        }
    }
}
