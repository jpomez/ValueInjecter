using System.Linq;

namespace Omu.ValueInjecter
{
    internal class SameNameType : ValueInjection
    {
        protected override void Inject(object source, object target)
        {
            var sourceProps = source.GetProps();
            for (var i = 0; i < sourceProps.Count(); i++)
            {
                var s = sourceProps.ElementAt(i);

                var t = target.GetProps().FirstOrDefault(prop => prop.Name == s.Name);
                if (t == null) continue;
                if (s.PropertyType != t.PropertyType) continue;

                t.SetValue(target, s.GetValue(source, null), null);
            }
        }
    }
}