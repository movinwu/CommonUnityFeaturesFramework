using INIParser.Attributes;
using INIParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace INIParser
{
    public class IniDeserializer
    {
        public T Deserialize<T>(IniFile ini, string section)
        {
            if (!ini.Sections.Select(x => x.Name).Contains(section))
                throw new ArgumentException($"Ini doesn't contain {section} section");

            var type = typeof(T);
            var obj = (T)Activator.CreateInstance(type);

            var objProps = obj.GetType().GetProperties()
                .Where(x => x.CanWrite &&
                !x.PropertyType.IsGenericType &&
                (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string))).ToArray();

            foreach (var prop in objProps)
            {
                var iniName = (IniPropertyName)prop.GetCustomAttributes(false).Where(x => x is IniPropertyName).FirstOrDefault();
                string propName = null;
                if (iniName != null)
                    propName = iniName.Name;
                else
                    propName = prop.Name;
                var value = ini[section][propName];
                if (value == null)
                    continue;

                var propType = prop.PropertyType;
                if (propType != typeof(string))
                {
                    var parameters = new object[] { value, null };

                    var method = propType.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null,
                        new Type[] { typeof(string), typeof(int).MakeByRefType() }, null);

                    var result = (bool)method.Invoke(null, parameters); 

                    if (!result)
                        continue;  

                    prop.SetValue(obj, parameters[1]);
                    continue;
                }
                prop.SetValue(obj, value);
            }
            return obj;

        }

    }
}
