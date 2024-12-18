using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Mirror : Trait
    {
        public Person Target;

        public T_Mirror(Person target)
        {
            Target = target;
        }

        public override string getName()
        {
            return $"Mirrored";
        }

        public override string getDesc()
        {
            return "This person is a perfect mirror of another. Their original identity is lost forever.";
        }

        public override void turnTick(Person p)
        {
            if (Target == null || p == Target || p == p.map.awarenessManager.chosenOne?.person || (p.unit != null && p.unit.isCommandable()))
            {
                p.traits.Remove(this);
                return;
            }

            p.extremeLikes.Clear();
            p.extremeLikes.AddRange(Target.extremeLikes);
            p.likes.Clear();
            p.likes.AddRange(Target.likes);
            p.hates.Clear();
            p.hates.AddRange(Target.hates);
            p.extremeHates.Clear();
            p.extremeHates.AddRange(Target?.extremeHates);
        }
    }
}
