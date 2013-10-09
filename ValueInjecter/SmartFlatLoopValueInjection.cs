using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Omu.ValueInjecter
{
    public class SmartFlatLoopValueInjection : LoopValueInjectionBase
    {
        private class Path
        {
            public Type Source { get; private set; }
            public Type Target { get; private set; }
            public IDictionary<string, Queue<string>> Pairs { get; private set; }

            public Path(Type source, Type target)
            {
                Target = target;
                Source = source;
                Pairs = new Dictionary<string, Queue<string>>();
            }
        }

        protected virtual bool Match(SmartConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name;
        }

        private static readonly IList<Path> Paths = new List<Path>();
        private static readonly IDictionary<Type, Type> WasLearned = new Dictionary<Type, Type>();


        protected override void Inject(object source, object target)
        {
            var sourceProps = source.GetProps();
            var targetProps = target.GetProps();

            if (!WasLearned.Contains(new KeyValuePair<Type, Type>(source.GetType(), target.GetType())))
            {
                lock (WasLearned)
                {
                    if (!WasLearned.Contains(new KeyValuePair<Type, Type>(source.GetType(), target.GetType())))
                    {
                        var match = Learn(source, target);
                        WasLearned.Add(source.GetType(), target.GetType());
                        if (match != null) Paths.Add(match);
                    }
                }
            }

            var pairs = Paths.Single(path => path.Source == source.GetType() && path.Target == target.GetType()).Pairs;
            foreach (var pair in pairs)
            {
                var clone = new Queue<string>(pair.Value);
#if !WindowsCE
                targetProps.GetByName(pair.Key).SetValue(target, Tunnelier.GetValue(clone, source));
#else
                targetProps.GetByName(pair.Key).SetValue(target, Tunnelier.GetValue(clone, source), null);
#endif
            }
        }

        private static Path Learn(object source, object target)
        {
            var targetProps = target.GetProps();
            var path = new Path(source.GetType(), target.GetType());
            foreach (PropertyInfo propertyDescriptor in targetProps)
            {
                var descriptor = propertyDescriptor;

                var trail = TrailFinder.GetTrails(propertyDescriptor.Name, source.GetType().GetInfos(),
                                                  type => TypesMatch(type, descriptor.PropertyType),
                                                  StringComparison.Ordinal).First();

                //var trail = TrailFinder.GetTrailsAndinjectValue(new PropertyWithComponent(target, propertyDescriptor),
                //                                                source.GetType().GetInfos().Select(
                //                                                    info =>
                //                                                    new PropertyWithComponent(source,
                //                                                                              new PropertyDescriptor(
                //                                                                                  info))));

                if (!trail.Any()) continue;
                //path.Pairs.Add(descriptor.Name, trail);
            }
            return path;
        }

        private static bool TypesMatch(Type sourceType, Type targetType)
        {
            return targetType == sourceType;
        }

        private static object SetValue(object sourcePropertyValue)
        {
            return sourcePropertyValue;
        }
    }


    public abstract class SmartFlatLoopValueInjection<TSourceProperty, TTargetProperty> : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyInfo t in target.GetProps())
            {
                if (t.PropertyType != typeof(TTargetProperty)) continue;

                var values = UberFlatter.Flat(t.Name, source, type => type == typeof(TSourceProperty)).ToList();

                if (!values.Any()) continue;

                var firstValue = values.First();

                if (firstValue == null) continue;

                var propertyValue = firstValue.GetValue();

                if (AllowSetValue(propertyValue))
                {
#if !WindowsCE
                    t.SetValue(target, SetValue((TSourceProperty)propertyValue));
#else
                    t.SetValue(target, SetValue((TSourceProperty)propertyValue), null);
#endif
                }
            }
        }

        protected abstract TTargetProperty SetValue(TSourceProperty sourceValues);

    }
}

