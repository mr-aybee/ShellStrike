using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShellStrikeTest
{
    public partial class Form1 : Form
    {
        SshClient SshClient { get; set; }
        public Form1()
        {
            InitializeComponent();


            /// ByPass Insecure Certificates
            ServicePointManager.ServerCertificateValidationCallback +=
                (s, certificate, chain, sslPolicyErrors)
                => true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            var keyboardAuthMethod = new KeyboardInteractiveAuthenticationMethod(txtUser.Text);
            keyboardAuthMethod.AuthenticationPrompt += (x, y) =>
            {
                foreach (var prompt in y.Prompts)
                {
                    if (prompt.Request.ToLowerInvariant().StartsWith("password"))
                        prompt.Response = txtPassword.Text;
                    if (prompt.Request.ToLowerInvariant().Contains("ssword"))
                        prompt.Response = txtPassword.Text;
                    if (prompt.Request.ToLowerInvariant().StartsWith("login as"))
                        prompt.Response = txtUser.Text;
                }
            };
            ConnectionInfo connectionInfo = new ConnectionInfo(
                txtIP.Text,
                txtUser.Text,
                new PasswordAuthenticationMethod(txtUser.Text, txtPassword.Text),
                keyboardAuthMethod
               );
            SshClient = new SshClient(connectionInfo);
            SshClient.Connect();
            lblStatus.Text = "Connected";
        }

        private void btnCommand_Click(object sender, EventArgs e)
        {
            var command = SshClient.CreateCommand(txtCommand.Text);
            command.CommandTimeout = TimeSpan.FromSeconds(30);
            var result = command.Execute();
            txtResponse.Text = result;

        }

        ShellStream shellStream { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            var modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            shellStream = SshClient.CreateShellStream("xterm", 255, 50, 800, 600, 4096, modes);
            lblStatus.Text += " " + "Stream Active";

            System.Threading.Thread.Sleep(2000);
            shellStream.WriteLine("");
            txtResponse.Text = shellStream.Read();
            shellStream.Flush();
        }

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //var promptRegex = new Regex(@"\][#$>%]");
            var promptRegex = new Regex(txtExpectation.Text);
            shellStream.WriteLine(txtCommand.Text);
            txtResponse.Text = shellStream.Expect(promptRegex);
            shellStream.Flush();
            //TryAgain:
            //    if (shellStream.DataAvailable)
            //        txtResponse.Text = shellStream.Read(); //shellStream.Expect(promptRegex);
            //    if (string.IsNullOrEmpty(txtResponse.Text))
            //        goto TryAgain;


        }
    }
}
