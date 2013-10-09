using System.Linq;

namespace Omu.ValueInjecter
{
    ///<summary>
    ///</summary>
    public abstract class ExactValueInjection : CustomizableValueInjection
    {
        protected override void Inject(object source, object target)
        {
            //var sp = source.GetProps().GetByName(sourcePref + SourceName());
            var sp = source.GetProps().FirstOrDefault(prop => prop.Name == sourcePref + SourceName());

            //var tp = target.GetProps().GetByName(SearchTargetName(TargetName()));
            var tp = target.GetProps().FirstOrDefault(prop => prop.Name == SearchTargetName(TargetName()));

            if (tp == null) return;

            if (!TypesMatch(sp.PropertyType, tp.PropertyType)) return;

            var value = sp.GetValue(source, null);
            if (AllowSetValue(value))
                tp.SetValue(target, SetValue(value), null);

        }

        public abstract string SourceName();

        public virtual string TargetName()
        {
            return SourceName();
        }
    }
}