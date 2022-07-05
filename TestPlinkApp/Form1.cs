using ShellStrike;
using ShellStrike.Card;
using ShellStrike.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TestPlinkApp
{
    public partial class Form1 : Form
    {
        PlinkHandler Plink { get; set; }
        Process process;
        public Form1()
        {
            InitializeComponent();
            Shown += (qq, ww) =>
            {




                Process process1 = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Program Files\\PuTTY\\plink.exe",
                    Arguments = "-ssh 192.168.8.112",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                process1.StartInfo = processStartInfo;
                process1.EnableRaisingEvents = true;
                process1.Start();
                //this.Text = process1.MainWindowTitle;
                var card = File.ReadAllText(@"C:\Users\malik\OneDrive\Desktop\LoginCard.xml");
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(card);
                foreach (XmlNode C in xDoc.SelectSingleNode("Root").ChildNodes)
                {
                    if (C.Name == "Command")
                    {
                        Command commandNode = new Command(C);
                        var wB = process1.StandardInput.Encoding.GetBytes(commandNode.Text);
                        process1.StandardInput.BaseStream.Write(wB, 0, wB.Length);
                        //process1.StandardInput.Write(commandNode.Text);
                        Invoke(new Action(delegate
                        {
                            richTextBox1.AppendText(commandNode.Text);
                        }));

                        var o = Output(process1.StandardOutput);
                        Invoke(new Action(delegate
                        {
                            richTextBox1.AppendText(o + " <EO>\r\n");
                        }));
                    }
                }

            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] b = UTF8Encoding.UTF8.GetBytes(txtCommand.Text);
            process.StandardInput.BaseStream.Write(b, 0, b.Length);
        }
        private void btnWithEnter_Click(object sender, EventArgs e)
        {
            byte[] b = UTF8Encoding.UTF8.GetBytes(txtCommand.Text + "\r\n");
            process.StandardInput.BaseStream.Write(b, 0, b.Length);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var os = process.StandardOutput;//.BaseStream;
            BufferedStream bufferedStream = new BufferedStream(os.BaseStream);
            byte[] b = new byte[1];
            var s = bufferedStream.ReadAsync(b, 0, 1).Wait(15000);
            stringBuilder.Append(UTF8Encoding.UTF8.GetString(b));
            if (!s) goto End;
            int readLen = (int)typeof(BufferedStream).GetField("_readLen", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bufferedStream);
            int readPos = (int)typeof(BufferedStream).GetField("_readPos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bufferedStream);
            if (readLen == 0) goto End;
            while (readPos < readLen)
            {
                bufferedStream.Read(b, 0, 1);
                stringBuilder.Append(UTF8Encoding.UTF8.GetString(b));
                readPos = (int)typeof(BufferedStream).GetField("_readPos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bufferedStream);
            }
        End:
            richTextBox1.AppendText(stringBuilder.ToString());
        }

        // Merged From linked CopyStream below and Jon Skeet's ReadFully example
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        string Output(StreamReader outputStream)
        {
            Thread.Sleep(1500);
            StringBuilder stringBuilder = new StringBuilder();
            //outputStream.
            char[] b = new char[1];
            int tries = 0;
            bool isStreamReading = false;
            while (isStreamReading == false && tries < 3)
            {
                isStreamReading = outputStream.ReadAsync(b, 0, 1).Wait(10000);
                stringBuilder.Append(b);
                bool isFetchable = isStreamReading;
                while (!isFetchable)
                {
                    try { var eOS = outputStream.EndOfStream; }
                    catch (Exception t)
                    {
                        Logger.Error(t);
                        continue;
                    }
                    isFetchable = true;
                }
                tries++;
            }
            if (!isStreamReading)
            {
                return stringBuilder.ToString();
            }
            int readLen = (int)typeof(StreamReader).GetField("charLen", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(outputStream);
            int readPos = (int)typeof(StreamReader).GetField("charPos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(outputStream);
            if (readLen == 0)
            {
                return stringBuilder.ToString();
            }
            while (readPos < readLen)
            {
                outputStream.Read(b, 0, 1);
                stringBuilder.Append(b);
                readPos = (int)typeof(StreamReader).GetField("charPos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(outputStream);
            }
            outputStream.DiscardBufferedData();
            return stringBuilder.ToString();
        }


        public static IEnumerable<Control> GetControls(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetControls(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

    }

}
