using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static partial class Reflect
    {
        public static class OnProperties
        {
            public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
            {
                return TypeSafeGetPropertyName(expression);
            }

            public static string TypeSafeGetPropertyName<TInput, TResult>(Expression<Func<TInput, TResult>> expression)
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
                var memberInfo = GetMemberInformation(expression);
                var info = memberInfo as PropertyInfo;
                return info.PropertyType;
            }

            public static MemberInfo GetMemberInformation<T>(Expression<Func<T, object>> propertyExpression)
            {
                var memberInformation = GetMemberInformation(propertyExpression.Body);
                return memberInformation;
            }

            public static MemberInfo GetMemberInformation(Expression propertyExpression)
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
                if (type == null)
                    return null;
                var runTimeProps = type.GetRuntimeProperties().ToList();
                if(runTimeProps.Any())
                    return runTimeProps.FirstOrDefault(p => p.Name == propertyName);
                return null;
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

            public static void SetProperty<TModel>(TModel instance, Expression<Func<TModel, object>> exp, object value)
            {
                var propertyName = GetPropertyName(exp);
                SetProperty(instance, propertyName, value);
            }

            public static void SetProperty<TModel>(TModel instance, string propertyName, object value)
            {
                var info = GetPropertyInformation(instance.GetType(), propertyName);

                var setMethod = info.GetSetMethod();
                if (setMethod != null)
                {
                    setMethod.Invoke(instance, new object[] { value });
                }
                else
                {
                    info.SetValue(instance, value);
                }                
            }

            public static void TypeSafeSetProperty<TModel, TPropertyType>(TModel instance, Expression<Func<TModel, TPropertyType>> exp, TPropertyType value)
            {
                var propertyName = TypeSafeGetPropertyName(exp);
                SetProperty(instance, propertyName, value);
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

        }
    }

}
