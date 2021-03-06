﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace Omu.ValueInjecter
{
    public abstract class SmartConventionInjection : ValueInjection
    {
        private class Path
        {
            public Type Source { get; set; }
            public Type Target { get; set; }
            public IDictionary<string, string> Pairs { get; set; }
        }

        protected abstract bool Match(SmartConventionInfo c);

        private static readonly IList<Path> paths = new List<Path>();
        private static readonly IDictionary<Type, Type> wasLearned = new Dictionary<Type, Type>();

        private Path Learn(object source, object target)
        {
            Path path = null;
            var sourceProps = source.GetProps();
            var targetProps = target.GetProps();
            var sci = new SmartConventionInfo
            {
                SourceType = source.GetType(),
                TargetType = target.GetType()
            };

            for (var i = 0; i < sourceProps.Count(); i++)
            {
                var s = sourceProps.ElementAt(i);
                sci.SourceProp = s;

                for (var j = 0; j < targetProps.Count(); j++)
                {
                    var t = targetProps.ElementAt(j);
                    sci.TargetProp = t;

                    if (!this.Match(sci)) continue;
                    if (path == null)
                        path = new Path
                        {
                            Source = sci.SourceType,
                            Target = sci.TargetType,
                            Pairs = new Dictionary<string, string> { { sci.SourceProp.Name, sci.TargetProp.Name } }
                        };
                    else path.Pairs.Add(sci.SourceProp.Name, sci.TargetProp.Name);
                }
            }
            return path;
        }

        protected override void Inject(object source, object target)
        {
            var sourceProps = source.GetProps();
            var targetProps = target.GetProps();

            if (!wasLearned.Contains(new KeyValuePair<Type, Type>(source.GetType(), target.GetType())))
            {
                lock (wasLearned)
                {
                    if (!wasLearned.Contains(new KeyValuePair<Type, Type>(source.GetType(), target.GetType())))
                    {

                        var match = this.Learn(source, target);
                        wasLearned.Add(source.GetType(), target.GetType());
                        if (match != null) paths.Add(match);
                    }
                }
            }

            var path = paths.SingleOrDefault(o => o.Source == source.GetType() && o.Target == target.GetType());

            if (path == null) return;

            foreach (var pair in path.Pairs)
            {
                var sp = sourceProps.GetByName(pair.Key);
                var tp = targetProps.GetByName(pair.Value);
                var setValue = true;
                var val = this.SetValue(ref setValue, new SmartValueInfo { Source = source, Target = target, SourceProp = sp, TargetProp = tp, SourcePropValue = sp.GetValue(source, null) });
                if (setValue) tp.SetValue(target, val, null);
            }
        }

        protected virtual object SetValue(ref bool setValue, SmartValueInfo info)
        {
            if (info.SourceProp.PropertyType.IsEnum)
                return (int)info.SourcePropValue;
            return info.SourcePropValue;
        }
    }

    public class SmartValueInfo
    {
        public PropertyInfo SourceProp { get; set; }
        public PropertyInfo TargetProp { get; set; }
        public object Source { get; set; }
        public object Target { get; set; }
        public object SourcePropValue { get; set; }
    }

    public class SmartConventionInfo
    {
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }

        public PropertyInfo SourceProp { get; set; }
        public PropertyInfo TargetProp { get; set; }
    }
}
