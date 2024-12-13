using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Curse_Mirror : Curse
    {
        public Person Target;

        public Curse_Mirror(Person target)
        {
            Target = target;
        }

        public override string getName()
        {
            if (Target == null)
            {
                return $"Familial Obsession (DefaultTarget)";
            }

            return $"Familial Obsession ({Target.getName()})";
        }

        public override string getDesc()
        {
            if (Target == null)
            {
                return $"This family is consumed by their pride in DefaultTarget. They strive to be like them in every way and their personilites twists unnaturally to the tune of their obsession.";
            }

            return $"This family is consumed by their pride in {Target.getName()}. They strive to be like them in every way and their personilites twists unnaturally to the tune of their obsession.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person || p == Target || (p.unit != null && p.unit.isCommandable()))
            {
                return;
            }

            T_Mirror mirror = (T_Mirror)p.traits.FirstOrDefault(t => t is T_Mirror);
            if (mirror == null)
            {
                mirror = new T_Mirror(Target);
                p.receiveTrait(mirror);
            }
        }
    }
}
