using INIParser.Exceptions;
using INIParser.Parser;
using System;
using System.IO;
using System.Linq;
using static INIParser.Parser.IniFile;

namespace INIParser
{
    public class IniDataParser
    {
        private IniFile iniFile { get; set;  }
        private string currentSection = null;
        private IniConfiguration configuration;

        public IniDataParser(IniConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IniDataParser() : this(new IniConfiguration())
        {
           
        } 
        public IniFile Parse(string iniFile)
        {
            this.iniFile = new IniFile();
            var IniLines = iniFile.Split("\n");

            for (int i = 0; i < IniLines.Length; i++)
            {
                try
                {
                    ParseLine(IniLines[i]);
                }
                catch (Exception e)
                {
                    if (configuration.SkipInvalidLines)
                        throw e;
                }
            }

            return this.iniFile;
        }
        public IniFile ParseFromFile(string path)
        {
            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                return Parse(file);
            }
            else
            {
                throw new FileNotFoundException("File doesn't exists");
            }
        }


        private void ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;
            if (ParseComment(line)) return;
            if (ParseSection(line)) return;
            if (ParseProperty(line)) return;

            throw new ParsingException($"Couldn't parse line: {line}.");
        }


        private bool ParseComment(string line)
        {
            if (line.Trim().StartsWith(configuration.CommentSymbol))
            {
                return true;
            }
            return false;
        }

        private bool ParseSection(string line)
        {
            var sectionStart = line.IndexOf("[");
            var sectionEnd = line.LastIndexOf("]");
            if (sectionStart == -1) return false;
            if (sectionEnd == -1) return false;

            var sectionRange = IniFile.Range.FromTo(sectionStart + 1, sectionEnd);

            var sectionName = line.SubstringWithRange(sectionRange);

            var section = new Section(sectionName);
            currentSection = sectionName;
            if (iniFile.Sections.Any(x => x.Name == sectionName))
                return false;
            iniFile.Sections.Add(section);
            return true;
        }


        private bool ParseProperty(string line)
        {
            var indexOfAssignmentChar = line.IndexOf(configuration.AssignmentSymbol);
            if (indexOfAssignmentChar == -1) return false;
            var keyRange = IniFile.Range.FromTo(0, indexOfAssignmentChar);
            var valueRange = IniFile.Range.FromTo(indexOfAssignmentChar + configuration.AssignmentSymbol.Length, line.Length);

            var key = line.SubstringWithRange(keyRange);
            var value = line.SubstringWithRange(valueRange);

            if (currentSection == null)
            {
                throw new NoSectionException($"You must put {key} into section");
            }
            iniFile.Sections.Where(x => x.Name == currentSection)
                .FirstOrDefault()
                .Properties.Add(
                new Property(key.Trim(), value.Trim()));

            return true;
        }


    }
}
