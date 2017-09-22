using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static partial class Reflect
    {
        public static class OnMethods
        {
            public static IEnumerable<MethodInfo> GetMethods<T>(string name)
            {
                return GetMethods(typeof(T));
            }

            public static IEnumerable<MethodInfo> GetMethods<T>(bool excludeContructor = false, bool excludeSpecials = false)
            {
                return GetMethods(typeof(T), excludeContructor, excludeSpecials);
            }
            
            public static IEnumerable<MethodInfo> GetMethods(Type type, bool excludeContructor = false, bool excludeSpecials = false)
            {
                Func<MethodInfo, bool> predicate = m => FilterMethodInfo(m, excludeContructor, excludeSpecials);
#if COREFX
                var methods =  type.GetTypeInfo().DeclaredMethods.Where(predicate);
                return methods;
#endif
#if NET
                var methods = type.GetMethods().Where(predicate);
                return methods;
#endif
                throw new NotImplementedException();
            }

            private static bool FilterMethodInfo(MethodInfo mi, bool excludeContructor, bool excludeSpecials)
            {
                if(mi.IsConstructor && excludeContructor)
                {
                    return false;
                }

                if(mi.IsSpecialName && excludeSpecials)
                {
                    return false;
                }

                return true;
            }

            public static MethodInfo GetMethod<T>(string name, params Type[] types)
            {
                return GetMethod(typeof(T), name, types);
            }

            public static MethodInfo GetMethod(Type type, string name, params Type[] types)
            {
#if COREFX
                var methods = type.GetTypeInfo().GetDeclaredMethods(name);
                var method = methods.FirstOrDefault(mi => MatchTypes(mi.GetParameters(), types));
                return method;
#endif
#if NET
                var method = types.Any() 
                    ? type.GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic, null, types, null) 
                    : type.GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                return method;
#endif
                throw new NotImplementedException();
            }

            private static bool MatchTypes(ParameterInfo[] parameterInfo, Type[] types)
            {
                for (int i = 0; i < parameterInfo.Length; i++)
                {
                    if(parameterInfo[i].ParameterType == types[i])
                    {
                        return true;
                    }
                }
                return false;
            }

            public static TResult Call<TImplement, TResult>(TImplement instance, string name, params object[] args)
            {
                return (TResult)Call(instance, name, args);
            }

            public static object Call(object instance, string name, params object[] args)
            {
                var type = instance.GetType();
                var types = args.Select(a => a.GetType()).ToArray();
                var method = GetMethod(type, name, types);
                var result = method.Invoke(instance, args);
                return result;
            }

            public static void VoidCall<TImplement>(TImplement instance, string name, params object[] args)
            {
                Call(instance, name, args);
            }

            public static void VoidCall(object instance, string name, params object[] args)
            {
                var type = instance.GetType();
                var types = args.Select(a => a.GetType()).ToArray();
                var method = GetMethod(type, name, types);
            }

            public static T ImplicitConvert<T>(object obj)
            {
                var toType = typeof(T);
                return (T)ImplicitConvert(toType, obj);
            }

            public static object ImplicitConvert(Type toType, object obj)
            {
                var argType = obj.GetType();

                var argMi = GetMethods(argType)
                    .Where(m => m.ReturnType == toType && MatchTypes(m.GetParameters(), new Type[] { argType })).FirstOrDefault();

                if (argMi != null)
                {
                    return argMi.Invoke(obj, new[] { obj });
                }

                var toMi = GetMethods(toType)
                    .Where(m => m.ReturnType == toType && MatchTypes(m.GetParameters(), new Type[] { argType })).FirstOrDefault();

                if (toMi != null)
                {
                    return toMi.Invoke(obj, new[] { obj });
                }

                throw new InvalidOperationException($"No implicit conversion exists on type {toType.Name} or {argType.Name}");
            }
        }
    }
}
