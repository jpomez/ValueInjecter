using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Omu.ValueInjecter
{
    public static class TrailFinder
    {
        public static IEnumerable<Queue<string>> GetTrails(string upn, IEnumerable<PropertyInfo> all, Predicate<Type> f, StringComparison comparison)
        {
            return all.SelectMany(p => GetTrails(upn, p, f, new Queue<string>(), comparison));
        }

        public static IEnumerable<Queue<string>> GetTrails(string upn, PropertyInfo prop, Predicate<Type> f, Queue<string> root, StringComparison comparison)
        {
            if (string.Equals(upn, prop.Name, comparison) && f(prop.PropertyType))
            {
                var queue = new Queue<string>();
                queue.Enqueue(prop.Name);
                yield return queue;
                yield break;
            }

            if (upn.StartsWith(prop.Name, comparison))
            {
                root.Enqueue(prop.Name);
                foreach (var pro in prop.PropertyType.GetInfos())
                {
                    foreach (var trail in GetTrails(upn.RemovePrefix(prop.Name, comparison), pro, f, root, comparison))
                    {
                        var queue = new Queue<string>();
                        queue.Enqueue(prop.Name);
                        foreach (var value in trail)
                        {
                            queue.Enqueue(value);
                        }
                        yield return queue;
                    }
                }
            }
        }

        public static IEnumerable<Queue<string>> GetTrailsAndinjectValue(PropertyWithComponent target, IEnumerable<PropertyWithComponent> all, Predicate<Type> f, StringComparison comparison)
        {
            return all.SelectMany(p => GetTrailsAndinjectValue(target, p, f, new Queue<string>(), comparison));
        }

        public static IEnumerable<Queue<string>> GetTrailsAndinjectValue(PropertyWithComponent target, PropertyWithComponent source, Predicate<Type> f, Queue<string> root, StringComparison comparison)
        {
            if (string.Equals(target.Property.Name, source.Property.Name, comparison) && f(source.Property.PropertyType))
            {
                var queue = new Queue<string>();
                queue.Enqueue(source.Property.Name);
                source.Property.SetValue(target.Component, source.Property.GetValue(source.Component, null), null);
                yield return queue;
                yield break;
            }

            if (target.Property.Name.StartsWith(source.Property.Name, comparison))
            {
                root.Enqueue(source.Property.Name);
                foreach (var pro in source.Property.PropertyType.GetInfos())
                {
                    foreach (var trail in GetTrails(target.Property.Name.RemovePrefix(source.Property.Name, comparison), pro, f, root, comparison))
                    {
                        var queue = new Queue<string>();
                        queue.Enqueue(source.Property.Name);
                        foreach (var value in trail.Reverse())
                        {
                            queue.Enqueue(value);
                        }
                        yield return queue;
                    }
                }
            }
        }
    }
}