using System;
using System.Collections.Generic;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static partial class Reflect
    {
        //TODO: Handle nested properties
        public static class OnMappings
        {
            public static void Map<T>(IDictionary<string, object> dictionary, T instance)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                foreach (var prop in instance.GetType().GetProperties(attr))
                {
                    if (prop.CanWrite)
                    {
                        if (dictionary.ContainsKey(prop.Name))
                        {
                            var v = Convert.ChangeType(dictionary[prop.Name], prop.PropertyType);
                            prop.SetValue(instance, v);
                        }
                    }
                }
            }

            public static IDictionary<string, object> TurnObjectIntoDictionary(object data)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                var dict = new Dictionary<string, object>();
                foreach (var prop in data.GetType().GetProperties(attr))
                {
                    if (prop.CanRead)
                    {
                        if (OnTypes.IsClass(prop.GetValue(data).GetType()) && !OnTypes.IsSimple(prop.GetValue(data).GetType()))
                            dict.Add(prop.Name, TurnObjectIntoDictionary(prop.GetValue(data)));
                        else
                            dict.Add(prop.Name, prop.GetValue(data, null));
                    }
                }
                return dict;
            }
        }
    }

}
