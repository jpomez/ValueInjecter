using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Omu.ValueInjecter
{
    /// <summary>
    /// this is for caching the PropertyDescriptorCollection and PropertyInfo[] for each Type
    /// </summary>
    public static class PropertyInfosStorage
    {
        private static readonly IDictionary<Type, IEnumerable<PropertyInfo>> Storage = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        private static readonly IDictionary<Type, IEnumerable<PropertyInfo>> InfosStorage = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        private static readonly object PropsLock = new object();
        private static readonly object InfosLock = new object();

        private static readonly IList<Action<Type>> Actions = new List<Action<Type>>();


        public static void RegisterActionForEachType(Action<Type> action)
        {
            Actions.Add(action);
        }

        public static IEnumerable<PropertyInfo> GetProps(Type type)
        {
            if (!Storage.ContainsKey(type))
            {
                lock (PropsLock)
                {
                    if (!Storage.ContainsKey(type))
                    {
                        if (!type.IsAnonymousType())
                            foreach (var action in Actions)
                                action(type);

                        //var props = type.GetRuntimeProperties();
                        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        Storage.Add(type, props);
                    }
                }
            }

            return Storage[type];
        }

        public static IEnumerable<PropertyInfo> GetProps(this object o)
        {
            return GetProps(o.GetType());
        }

        public static IEnumerable<PropertyInfo> GetInfos(this Type type)
        {
            if (!InfosStorage.ContainsKey(type))
            {
                lock (InfosLock)
                {
                    if (!InfosStorage.ContainsKey(type))
                    {
                        if (!type.IsAnonymousType())
                            foreach (var action in Actions)
                                action(type);

                        //var props = type.GetRuntimeProperties()
                        //                .Union(type
                        //                .GetTypeInfo().ImplementedInterfaces
                        //                .SelectMany(t => t.GetRuntimeProperties()));
                        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                        var props = type.GetProperties(bindingFlags).Union(type.GetInterfaces().SelectMany(t => t.GetProperties(bindingFlags)));

                        InfosStorage.Add(type, props);
                    }
                }
            }
            return InfosStorage[type];
        }

        public static IEnumerable<PropertyInfo> GetInfos(this object o)
        {
            return GetInfos(o.GetType());
        }
    }
}