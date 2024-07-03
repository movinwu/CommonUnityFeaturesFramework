using System;
using System.Collections.Generic;
using System.Text;

namespace INIParser
{
    public class IniConfiguration
    {
        public string CommentSymbol { get; set; } = "#";
        public string AssignmentSymbol { get; set; } = "=";
        public bool SkipInvalidLines { get; set; } = false;
        public bool IgnoreAttributes { get; set; } = false;
    }
}
