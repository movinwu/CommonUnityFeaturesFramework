using INIParser.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INIParser
{
    public class IniSerializer
    {
        public string Serialize(object obj, IniConfiguration config)
        {
            var objProps = obj.GetType().GetProperties()
                .Where(x => x.CanRead &&
                !x.PropertyType.IsGenericType &&
                (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string)));

            var serialized = string.Empty;
            serialized += $"[{obj.GetType().Name}]\n";
            foreach (var item in objProps)
            {
                string name = item.Name;
                if (!config.IgnoreAttributes)
                {
                    var iniName = (IniPropertyName)item.GetCustomAttributes(false).Where(x => x is IniPropertyName).FirstOrDefault();
                    if (iniName != null)
                        name = iniName.Name;
                }
                serialized += $"{name} {config.AssignmentSymbol} {item.GetValue(obj)}\n";
            }
            return serialized;

        }
        public string Serialize(object obj)
        {
            return Serialize(obj, new IniConfiguration());
        }
    }
}
