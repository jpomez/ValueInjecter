using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Omu.ValueInjecter
{
    public class FlatLoopValueInjection : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyInfo t in target.GetProps())
            {
                var t1 = t;
                var es = UberFlatter.Flat(t.Name, source, type => TypesMatch(type, t1.PropertyType)).ToList();

                if (!es.Any()) continue;
                var endpoint = es.First();
                if(endpoint == null) continue;
                var val = endpoint.Property.GetValue(endpoint.Component, null);

                if (AllowSetValue(val))
                    t.SetValue(target, SetValue(val), null);
            }
        }

        protected virtual bool TypesMatch(Type sourceType, Type targetType)
        {
            return targetType == sourceType;
        }

        protected virtual object SetValue(object sourcePropertyValue)
        {
            return sourcePropertyValue;
        }
    }


    public abstract class FlatLoopValueInjection<TSourceProperty, TTargetProperty> : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyInfo t in target.GetProps())
            {
                if (t.PropertyType != typeof(TTargetProperty)) continue;

                var values = UberFlatter.Flat(t.Name, source, type => type == typeof(TSourceProperty)).ToList();

                if (!values.Any()) continue;

                var val = values.First().Property.GetValue(values.First().Component, null);

                if (AllowSetValue(val))
                    t.SetValue(target, SetValue((TSourceProperty)val), null);
            }
        }

        protected abstract TTargetProperty SetValue(TSourceProperty sourceValues);

    }
}