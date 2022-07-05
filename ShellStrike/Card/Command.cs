using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShellStrike.Card
{
    public class Command
    {
        public string Text { get; set; }
        public string CommandMode { get; set; }
        public string Condition { get; set; }
        public int WaitForOutput { get; set; } = 3000;
        public int CommandTimeout { get; set; } = 3000;
        public XmlNodeList ChildNodes { get; set; }


        public Command(string text, string commandMode, string condition, int waitForOutput, int commandTimeout = 3000)
        {
            Text = text;
            CommandMode = commandMode;
            Condition = condition;
            if (commandTimeout == 0)
                this.CommandTimeout = 3000;
        }

        public Command(string text, string commandMode, string condition, int waitForOutput, int commandTimeout, XmlNodeList childNodes)
        {
            Text = text;
            CommandMode = commandMode;
            Condition = condition;
            if (commandTimeout == 0)
                this.CommandTimeout = 3000;
            ChildNodes = childNodes;
        }

        public Command(XmlNode Node)
        {
            Text = Node.GetAttributeValue("Text");
            CommandMode = Node.GetAttributeValue("CommandMode");
            Condition = Node.GetAttributeValue("Condition");
            CommandTimeout = Node.GetAttributeValue("CommandTimeout").ToInt32();
            WaitForOutput = Node.GetAttributeValue("WaitForOutput").ToInt32();
            Condition = Node.GetAttributeValue("Condition");
            ChildNodes = Node.ChildNodes;
        }


    }
}
