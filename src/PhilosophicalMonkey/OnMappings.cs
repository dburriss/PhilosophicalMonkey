using System;
using System.Collections.Generic;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static partial class Reflect
    {
        public static class OnMappings
        {
            
            public static TTo Map<TTo>(IDictionary<string, object> dictionary)
            {
                var result = Activator.CreateInstance<TTo>();
                Map(dictionary, result);
                return result;
            }

            public static void Map<TFrom, TTo>(TFrom from, TTo to)
            {
                if (OnTypes.IsAssignable(typeof(TFrom), typeof(IDictionary<string, object>)))
                {
                    Map((IDictionary<string, object>) from, to);
                }
                else
                {
                    var data = OnMappings.TurnObjectIntoDictionary(from);
                    OnMappings.Map(data, to);
                }
            }

            public static void Map<TTo>(IDictionary<string, object> dictionary, TTo instance)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                foreach (var prop in instance.GetType().GetProperties(attr))
                {
                    if (prop.CanWrite && dictionary.ContainsKey(prop.Name))
                    {
                        var propType = prop.PropertyType;
                        var dataValue = dictionary[prop.Name];
                        if (OnTypes.IsSimple(propType))
                        {
                            var v = Convert.ChangeType(dataValue, propType);
                            prop.SetValue(instance, v);
                        }
                        else
                        {
                            //TODO:when types match or are convertable, just assign
                            if (false)
                            {
                            }
                            //when corresponding prop name in the dictionary of complex type is a IDictionary<string, object>
                            if (dataValue is IDictionary<string, object>)
                            {
                                var v = TurnDictionaryIntoObject((IDictionary<string, object>)dataValue, propType);
                                prop.SetValue(instance, v);
                            }
                        }
                    }
                }
            }

            public static object TurnDictionaryIntoObject(IDictionary<string, object> dictionary, Type type)
            {
                var result = Activator.CreateInstance(type);
                Map(dictionary, result);
                return result;
            }

            public static TTo TurnDictionaryIntoObject<TTo>(IDictionary<string, object> dictionary)
            {
                return Map<TTo>(dictionary);
            }

            public static IDictionary<string, object> TurnObjectIntoDictionary(object data)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                var dict = new Dictionary<string, object>();
                foreach (var prop in data.GetType().GetProperties(attr))
                {
                    if (prop.CanRead)
                    {
                        var propValue = prop.GetValue(data);
                        if (propValue != null && OnTypes.IsClass(propValue.GetType()) && !OnTypes.IsSimple(propValue.GetType()))
                        {
                            dict.Add(prop.Name, TurnObjectIntoDictionary(propValue));
                        }
                        else
                        {
                            dict.Add(prop.Name, prop.GetValue(data, null));
                        }
                            
                    }
                }
                return dict;
            }
        }
    }

}
