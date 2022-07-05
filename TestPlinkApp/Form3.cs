using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestPlinkApp
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectionInfo connection = new ConnectionInfo("192.168.8.112", "root", new AuthenticationMethod[] { new PasswordAuthenticationMethod("root", "Malik@1234") });
            
            SshClient sshClient = new SshClient(connection);
            sshClient.Connect();


        }
    }
}
