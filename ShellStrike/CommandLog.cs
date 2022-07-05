using System;
using System.Text;
using System.Xml.Serialization;

namespace ShellStrike
{
    [Serializable]
    public class CommandLog
    {
        [XmlAttribute]
        public DateTime CommandTime { get; set; }
        [XmlAttribute]
        public CommandDirection CommandDirection { get; set; }
        [XmlAttribute]
        public CommandType CommandType { get; set; } //  SQL-Condition-Text-Out[BC]-
        [XmlAttribute]
        public string Text { get; set; }

        [XmlIgnore]
        public string Response
        {
            get => Encoding.UTF8.GetString(Convert.FromBase64String(ResponseValTemp));
            set => ResponseValTemp = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        [XmlAttribute(AttributeName = "Response")]
        public string ResponseValTemp { get; set; }

        public CommandLog(CommandDirection commandDirection, CommandType commandType, string text, string response, string ExecutionCode = "")
        {
            CommandTime = DateTime.Now;
            CommandDirection = commandDirection;
            CommandType = commandType;
            Text = text;
            Response = response;
        }

        public CommandLog() { }
    }
}

public enum CommandType { SQLCommand, CommandText, SQLCondition, BreakCase, BreakCaseError, ProcessAfterSQL, ProcessBeforeSQL, Renci }

public enum CommandDirection { INPUT, OUTPUT, PROCESS }