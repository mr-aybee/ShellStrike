using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ShellStrike
{
    [Serializable]
    [TypeConverter(typeof(PairConverter))]
    public class Pair
    {


        [XmlAttribute]
        public string Name { get; set; }

        [TypeConverter(typeof(ValueConverter))]
        public object Value { get; set; }


        public Pair() { }


        public Pair(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public Pair(string name, string value)
        {
            Name = name;
            Value = value;
        }




        public Pair(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public Pair(string name, long value)
        {
            Name = name;
            Value = value;
        }

        public Pair(string name, double value)
        {
            Name = name;
            Value = value;
        }

        public Pair(string name, DateTime value)
        {
            Name = name;
            Value = value;
        }

        public Pair(string name, bool value)
        {
            Name = name;
            Value = value;
        }

    }


    public class PairConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        => true;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        => true;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            try
            {
                if (value?.ToString()?.Contains("[]") == true)
                {
                    string input = value?.ToString();

                    Pair p = new Pair
                    {
                        Name = Regex.Match(input, @"(.+?) ,", RegexOptions.Multiline).Value,
                        Value = Regex.Match(input, @" , (.*)", RegexOptions.Multiline).Value
                    };
                    return p;
                }
            }
            catch { }
            return new Pair();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value?.GetType().Name == "Pair")
            { return ((Pair)value).Name + " , " + ((Pair)value).Value?.ToString(); }
            return "";
        }


    }


    public class ValueConverter : StringConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        => true;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        => true;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        => value?.ToString();


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        => value?.ToString();



    }

}
