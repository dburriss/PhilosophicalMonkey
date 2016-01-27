using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ChimpLab.PhilosophicalMonkey
{
    public static class Reflect
    {

        private static IEnumerable<Type> GetInterfaces(Type type)
        {
#if DOTNET5_4
                return type.GetTypeInfo().ImplementedInterfaces;
#endif
#if NET451
            return type.GetInterfaces().AsEnumerable();
#endif
            throw new NotImplementedException();
        }

        private static bool IsAbstract(Type type)
        {
#if DOTNET5_4
            return type.GetTypeInfo().IsAbstract;
#endif
#if NET451
            return type.IsAbstract;
#endif
        }

        private static bool IsInterface(Type type)
        {
#if DOTNET5_4
            return type.GetTypeInfo().IsInterface;
#endif
#if NET451
            return type.IsInterface;
#endif
        }

        private static bool IsGenericType(Type type)
        {
#if DOTNET5_4
            return type.GetTypeInfo().IsGenericType;
#endif
#if NET451
            return type.IsGenericType;
#endif
        }

        public static IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
#if DOTNET5_4
                yield return type.GetTypeInfo().Assembly;
#endif
#if NET451
                yield return type.Assembly;
#endif
            }
        }

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired = false) where T : Attribute
        {
            Type t = typeof(T);
            var attribute = member.GetCustomAttribute(t, false);

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture,
                        "The {0} attribute is required on member {1}",
                        t.Name,
                        member.Name));
            }

            return (T)attribute;
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        public static Type GetPropertyType<T>(Expression<Func<T, object>> expression)
        {
            var info = GetPropertyInformation(expression) as PropertyInfo;
            return info.PropertyType;
        }

        public static MemberInfo GetPropertyInformation<T>(Expression<Func<T, object>> propertyExpression)
        {
            return PropertyInformation(propertyExpression.Body);
        }

        public static MemberInfo PropertyInformation(Expression propertyExpression)
        {
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null)
            {
                var propertyMember = memberExpr.Member as PropertyInfo;
                if (propertyMember != null)
                    return memberExpr.Member;
            }

            return null;
        }

        public static MemberInfo GetMemberInformation(Type type, string propertyName)
        {
            return type.GetRuntimeProperties().FirstOrDefault(p => p.Name == propertyName);
        }

        public static PropertyInfo GetPropertyInformation(Type type, string propertyName)
        {
            return type.GetRuntimeProperties().FirstOrDefault(p => p.Name == propertyName);
        }

        public static PropertyInfo GetPropertyInfoFromPath<T>(string path)
        {
            var type = typeof(T);
            return GetPropertyInfoFromPath(type, path);
        }

        public static PropertyInfo GetPropertyInfoFromPath(Type type, string path)
        {
            var parts = path.Split('.');
            var count = parts.Count();

            if (count == 0)
                throw new InvalidOperationException("Not a valid path");

            if (count == 1)
                return GetPropertyInformation(type, parts.First());
            else
            {
                var t = GetPropertyInformation(type, parts[0]).PropertyType;
                return GetPropertyInfoFromPath(t, string.Join(".", parts.Skip(1)));
            }
        }


        public static TResult NullSafeGetValue<TSource, TResult>(TSource source, Expression<Func<TSource, TResult>> expression, TResult defaultValue)
        {
            var value = GetValue(source, expression);
            return value == null ? defaultValue : (TResult)value;
        }

        public static TCastResultType NullSafeGetValue<TSource, TResult, TCastResultType>(TSource source, Expression<Func<TSource, TResult>> expression, TCastResultType defaultValue, Func<object, TCastResultType> convertToResultToAction)
        {
            var value = GetValue(source, expression);
            return value == null ? defaultValue : convertToResultToAction.Invoke(value);
        }

        public static string GetFullPropertyPathName<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
        {
            string expressionBodyString = expression.Body.ToString();

            if (expressionBodyString.Contains("Convert("))
            {
                expressionBodyString = expressionBodyString.Replace("Convert(", string.Empty);
                expressionBodyString = expressionBodyString.Substring(0, expressionBodyString.Length - 1);
            }

            string path = expressionBodyString.Replace(expression.Parameters[0] + ".", string.Empty);
            return path;
        }

        public static object GetValue<TSource, TResult>(TSource source, Expression<Func<TSource, TResult>> expression)
        {
            string fullPropertyPathName = GetFullPropertyPathName(expression);
            return GetNestedPropertyValue(fullPropertyPathName, source);
        }

        public static object GetNestedPropertyValue(string name, object obj)
        {
            PropertyInfo info;
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }
                Type type = obj.GetType();
                if (obj is IEnumerable)
                {
                    type = (obj as IEnumerable).GetType();
                    var methodInfo = type.GetRuntimeMethods().Single(m => m.Name == "get_Item");
                    var index = int.Parse(part.Split('(')[1].Replace(")", string.Empty));
                    try
                    {
                        obj = methodInfo.Invoke(obj, new object[] { index });
                    }
                    catch (Exception)
                    {
                        obj = null;
                    }
                }
                else
                {
                    info = type.GetRuntimeProperties().SingleOrDefault(p => p.Name == part);
                    if (info == null)
                    {
                        return null;
                    }
                    obj = info.GetValue(obj, null);
                }
            }
            return obj;
        }

        public static IEnumerable<Type> GetTypesFromNamespace(Assembly assembly, params string[] @namespaces)
        {
            return assembly.GetTypes().Where(type => @namespaces.Contains(type.Namespace));
        }

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
                    dict.Add(prop.Name, prop.GetValue(data, null));
                }
            }
            return dict;
        }
    }
}
