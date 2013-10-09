using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Omu.ValueInjecter
{
    public static class Tunnelier
    {
        //public static PropertyWithComponent Digg(IList<string> trail, object o)
        //{
        //    if (trail.Count == 1)
        //    {
        //        var prop = o.GetProps().FirstOrDefault(p => p.Name == trail[0]);
        //        return new PropertyWithComponent { Component = o, Property = prop };
        //    }
        //    else
        //    {
        //        var prop = o.GetProps().FirstOrDefault(p => p.Name == trail[0]);

        //        if (prop.GetValue(o) == null) prop.SetValue(o, Activator.CreateInstance(prop.PropertyType));

        //        var val = prop.GetValue(o);

        //        trail.RemoveAt(0);
        //        return Digg(trail, val);
        //    }
        //}

        //public static PropertyWithComponent GetPropertyWithComponent(IList<string> trail, object o)
        //{
        //    if (trail.Count == 1)
        //    {
        //        var prop = o.GetProps().FirstOrDefault(p => p.Name == trail[0]);
        //        return new PropertyWithComponent { Component = o, Property = prop };
        //    }

        //    var propx = o.GetProps().FirstOrDefault(p => p.Name == trail[0]);
        //    var val = propx.GetValue(o);
        //    if (val == null) return null;
        //    trail.RemoveAt(0);
        //    return GetPropertyWithComponent(trail, val);
        //}

        public static PropertyWithComponent Digg(Queue<string> trail, object o)
        {
            PropertyInfo prop;

            if (trail.Count == 1)
            {
                prop = o.GetProps().GetByName(trail.Dequeue());
                return new PropertyWithComponent(o, prop);
            }

            prop = o.GetProps().GetByName(trail.Dequeue());

            if (prop.GetValue(o, null) == null) prop.SetValue(o, Activator.CreateInstance(prop.PropertyType), null);

            var val = prop.GetValue(o, null);

            return Digg(trail, val);
        }

        public static PropertyWithComponent GetPropertyWithComponent(Queue<string> trail, object o)
        {
            PropertyInfo prop;

            if (trail.Count == 1)
            {
                prop = o.GetProps().GetByName(trail.Dequeue());
                return new PropertyWithComponent(o, prop);
            }

            prop = o.GetProps().GetByName(trail.Dequeue());
            var val = prop.GetValue(o, null);
            return val == null ? null : GetPropertyWithComponent(trail, val);
        }

        public static PropertyWithComponent GetPropertyWithComponent(Queue<PropertyInfo> trail, object o)
        {
            if (trail.Count == 1)
            {
                return new PropertyWithComponent(o, trail.Dequeue());
            }

            var val = trail.Dequeue().GetValue(o, null);
            return val == null ? null : GetPropertyWithComponent(trail, val);
        }

        public static object GetValue(Queue<string> trail, object o)
        {
            PropertyInfo prop;

            if (trail.Count == 1)
            {
                prop = o.GetProps().GetByName(trail.Dequeue());
                return prop.GetValue(o, null);
            }

            prop = o.GetProps().GetByName(trail.Dequeue());
            var val = prop.GetValue(o, null);
            return val == null ? null : GetValue(trail, val);
        }


    }
}