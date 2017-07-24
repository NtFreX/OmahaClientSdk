using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Omaha
{
    //This Attribute is used in the diffrent projects to identify the methods wich deliver system infos
    //The method must have no parameter, must return an object and must be static
    [AttributeUsage(AttributeTargets.Class)]
    public class SystemInfoAttribute : Attribute
    {
        public static void ReadSystemInfo(Type type, ref dynamic systemInfo, bool ignorgAttribute = false, object instance = null)
        {
            if (!ignorgAttribute && !type.GetCustomAttributes<SystemInfoAttribute>().Any()) return;

            string name = $"{type.FullName}";
            dynamic typeInfo = systemInfo, newTypeInfo = null;
            foreach (var part in name.Split('.'))
            {
                if (((IDictionary<string, Object>)typeInfo).ContainsKey(part))
                {
                    if (((IDictionary<string, Object>)typeInfo)[part] is ExpandoObject)
                    {
                        typeInfo = ((IDictionary<string, Object>)typeInfo)[part];
                        continue;
                    }
                    var value = ((IDictionary<string, Object>)typeInfo)[part];
                    newTypeInfo = new ExpandoObject();
                    ((IDictionary<string, Object>)typeInfo)[part] = newTypeInfo;
                    typeInfo = newTypeInfo;
                    ((IDictionary<string, Object>)typeInfo).Add("SameNameAsClassProperty", value);
                    continue;
                }
                newTypeInfo = new ExpandoObject();
                ((IDictionary<string, Object>)typeInfo).Add(part, newTypeInfo);
                typeInfo = newTypeInfo;
            }
            if (newTypeInfo == null) return;
            foreach (var field in type.GetFields())
            {
                var propertyName = field.Name;
                var nameCounter = 1;
                while ((newTypeInfo as IDictionary<string, Object>).ContainsKey(propertyName))
                { propertyName = field.Name + nameCounter++; }

                try { (newTypeInfo as IDictionary<string, Object>).Add(propertyName, field.GetValue(instance)); }
                catch { /*ignore*/ } //when the field is not static and instance is null
            }

            foreach (var property in type.GetProperties())
            {
                var propertyName = property.Name;
                var nameCounter = 1;
                while ((newTypeInfo as IDictionary<string, Object>).ContainsKey(propertyName))
                { propertyName = property.Name + nameCounter++; }

                try { (newTypeInfo as IDictionary<string, Object>).Add(propertyName, property.GetValue(instance)); }
                catch { /*ignore*/ } //when the field is not static and instance is null
            }
        }
    }
}
