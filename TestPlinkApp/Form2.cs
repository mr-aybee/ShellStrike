using CliWrap;
using CliWrap.Buffered;
using ShellStrike.Card;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TestPlinkApp
{
    public partial class Form2 : Form
    {
        Process process1;
        public Form2()
        {
            InitializeComponent();
            this.Shown += (x, y) =>
            {
                //process1 = new Process();
                //ProcessStartInfo processStartInfo = new ProcessStartInfo
                //{
                //    FileName = "C:\\Program Files\\PuTTY\\plink.exe",
                //    Arguments = "-ssh 192.168.8.112",
                //    RedirectStandardError = true,
                //    RedirectStandardInput = true,
                //    RedirectStandardOutput = true,
                //    CreateNoWindow = true,
                //    UseShellExecute = false
                //};
                //process1.StartInfo = processStartInfo;
                //process1.EnableRaisingEvents = true;
                //process1.Start();
            };
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            MemoryStream inputMemoryStream = new MemoryStream();
            MemoryStream outputMemoryStream = new MemoryStream();

            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        while(outputMemoryStream.pe)


            //    }
            //}).Start();

            var result = await Cli.Wrap("C:\\Program Files\\PuTTY\\plink.exe")
            .WithArguments("-ssh 192.168.8.112")
            .WithWorkingDirectory("")
            .WithStandardOutputPipe(PipeTarget.ToStream(outputMemoryStream, true))
            .WithStandardInputPipe(PipeSource.FromStream(inputMemoryStream, true))
            .ExecuteAsync();




        }

        void Output()
        {

        }
    }
}
