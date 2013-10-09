using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Omu.ValueInjecter
{
    public class UnflatLoopValueInjection : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyInfo sourceProp in source.GetProps())
            {
                var prop = sourceProp;
                var endpoints = UberFlatter.Unflat(sourceProp.Name, target, t => TypesMatch(prop.PropertyType, t)).ToList();

                if(!endpoints.Any()) continue;

                var value = sourceProp.GetValue(source, null);

                if (AllowSetValue(value))
                    foreach (var endpoint in endpoints)
                        endpoint.Property.SetValue(endpoint.Component, SetValue(value), null);
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

    public abstract class UnflatLoopValueInjection<TSourceProperty, TTargetProperty> : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyInfo sourceProp in source.GetProps())
            {
                if(sourceProp.PropertyType != typeof(TSourceProperty)) continue;
                var endpoints = UberFlatter.Unflat(sourceProp.Name, target, t => t == typeof(TTargetProperty)).ToList();
                if (!endpoints.Any()) continue;
                var value = sourceProp.GetValue(source, null);

                if (AllowSetValue(value))
                    foreach (var endpoint in endpoints)
                        endpoint.Property.SetValue(endpoint.Component, SetValue((TSourceProperty)value), null);
            }
        }

        protected abstract TTargetProperty SetValue(TSourceProperty sourcePropertyValue);

    }
}