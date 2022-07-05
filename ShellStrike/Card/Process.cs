using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShellStrike.Card
{
    public class xProcess
    {
        public string Type { get; set; } = "SQL";
        public string AfterSQL { get; set; }
        public string BeforeSQL { get; set; }
        public XmlNodeList pNodes { get; set; }

        public xProcess(string type, string afterSQL, string beforeSQL, XmlNodeList pnodes)
        {
            Type = type;
            AfterSQL = afterSQL;
            BeforeSQL = beforeSQL;
            this.pNodes = pnodes;
        }
        public xProcess(XmlNode node)
        {
            Type = node.GetAttributeValue("Type");
            AfterSQL = node.GetAttributeValue("AfterSQL");
            BeforeSQL = node.GetAttributeValue("BeforeSQL");
            this.pNodes = node.ChildNodes;
        }
    }
}
