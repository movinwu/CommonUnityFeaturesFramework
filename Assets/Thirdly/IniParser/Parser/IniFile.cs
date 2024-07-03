using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INIParser.Parser
{
    public class IniFile
    {
        public List<Section> Sections { get; }
        public IniFile()
        {
            Sections = new List<Section>();
        }

        public Section this[string section]
        {
            get => Sections.FirstOrDefault(x => x.Name == section);
        }

        public class Section
        {
            public string Name { get; set; }
            public List<Property> Properties { get; private set; }
            public Section(string name)
            {
                Name = name;
                Properties = new List<Property>();
            }
            public object this[string property]
            {
                get => Properties.FirstOrDefault(x => x.Key == property)?.Value;
            }

        }

        public class Property
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public Property(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }


        public struct Range
        {
            int start;
            int length;

            public int Start
            {
                get
                {
                    return start;
                }

                set
                {
                    start = value < 0 ? 0 : value;
                }
            }

            public int Length
            {
                get
                {
                    return length;
                }

                set
                {
                    length = value < 0 ? 0 : value;
                }
            }

            public int End
            {
                get
                {
                    return length <= 0 ? 0 : start + (length - 1);
                }
            }

            public bool IsEmpty { get { return length == 0; } }


            public static Range FromTo(int start, int end)
            {
                if (end - start < 0 || start < 0 || end < 0)
                {
                    return new Range();
                }

                return new Range { Start = start, Length = end - start + 1 };
            }
            public static Range WithLength(int start, int length)
            {
                if (length <= 0 || start < 0) 
                    return new Range();
                return new Range { Start = start, Length = length };
            }
        }

    }
    public static class StringExtension
    {
        public static string SubstringWithRange(this string str, IniFile.Range range)
        {
            return str.Substring(range.Start, range.End - range.Start);
        }
    }
}
