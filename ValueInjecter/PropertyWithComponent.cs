using System.Reflection;

namespace Omu.ValueInjecter
{
    public class PropertyWithComponent
    {
        public PropertyInfo Property;
        public object Component;

        public PropertyWithComponent(object o, PropertyInfo prop)
        {
            this.Property = prop;
            this.Component = o;
        }

        public object GetValue()
        {
            return Property != null ? Property.GetValue(Component, null) : null;
        }
    }
}