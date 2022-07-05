using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public static class XmlExtensions
{

    public static string GetAttributeValue(this XmlNode node, string AttributeName)
    {
        try
        {
            return node.Attributes[AttributeName]?.Value;
        }
        catch { }
        return "";
    }
}
