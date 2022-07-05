using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShellStrike.Card
{
    public class BreakCase
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Regex { get; set; }
        public string EndWith { get; set; }
        public XmlNodeList ChildNodes { get; set; }



        public BreakCase() { }
        public BreakCase(string name, string text)
        {
            Name = name;
            Text = text;
        }
        public BreakCase(string name, string text, XmlNodeList childNodes)
        {
            Name = name;
            Text = text;
            ChildNodes = childNodes;
        }
        public BreakCase(XmlNode bNode)
        {
            Name = bNode.GetAttributeValue("Name");
            Text = bNode.GetAttributeValue("Text");
            Regex = bNode.GetAttributeValue("Regex");
            EndWith = bNode.GetAttributeValue("EndWith");
            ChildNodes = bNode.ChildNodes;
        }
    }
}
