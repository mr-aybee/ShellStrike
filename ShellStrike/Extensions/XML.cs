using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

public static class XML
{

    public static string SerializeObject(object toSerialize)
    {
        //XmlDocument myXml = new XmlDocument();
        //XPathNavigator xNav = myXml.CreateNavigator();
        //XmlSerializer x = new XmlSerializer(value.GetType());
        //using (XmlWriter xs = xNav.AppendChild())
        //{
        //    x.Serialize(xs, value);
        //}
        //return myXml.OuterXml;
        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }

    public static string AsXml<T>(this T toSerialize)
    {
        return SerializeObject(toSerialize);
        //XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

        //using (StringWriter textWriter = new StringWriter())
        //{
        //    xmlSerializer.Serialize(textWriter, toSerialize);
        //    return textWriter.ToString();
        //}
    }


    public static string SerializeObjectIndented(object value)
    {
        XmlDocument myXml = new XmlDocument();
        XPathNavigator xNav = myXml.CreateNavigator();
        XmlSerializer x = new XmlSerializer(value.GetType());
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = ("\t");
        settings.OmitXmlDeclaration = true;
        using (XmlWriter xs = xNav.AppendChild())
        {
            x.Serialize(xs, value);
        }
        return myXml.OuterXml;
    }

    public static T DeserializeObject<T>(string value)
    {
        XmlSerializer x = new XmlSerializer(typeof(T));
        return (T)x.Deserialize(new StringReader(value)); ;
    }

    public static string PrettyXML(string xml)
    {
        string result = "";
        MemoryStream mStream = new MemoryStream();
        XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
        XmlDocument document = new XmlDocument();

        try
        {
            // Load the XmlDocument with the XML.
            document.LoadXml(xml);
            writer.Formatting = Formatting.Indented;
            // Write the XML into a formatting XmlTextWriter
            document.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();
            // Have to rewind the MemoryStream in order to read
            // its contents.
            mStream.Position = 0;
            // Read MemoryStream contents into a StreamReader.
            StreamReader sReader = new StreamReader(mStream);
            // Extract the text from the StreamReader.
            string formattedXml = sReader.ReadToEnd();
            result = formattedXml;
        }
        catch (XmlException)
        {
            // Handle Exception
        }

        mStream.Close();
        writer.Close();
        mStream.Dispose();
        return result;
    }


}

