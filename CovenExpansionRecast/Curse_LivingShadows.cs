using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Curse_LivingShadows : Curse
    {
        public override string getName()
        {
            return "Living Shadows";
        }

        public override string getDesc()
        {
            return "The sins of this family have manifested themselves as a living spiteful thing. It watches over them, guiding them to greater ruin.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person || (p.unit != null && p.unit.isCommandable()))
            {
                return;
            }

            T_LivingShadow shadow = (T_LivingShadow)p.traits.FirstOrDefault(t => t is T_LivingShadow);
            if (shadow == null)
            {
                shadow = new T_LivingShadow();
                p.receiveTrait(shadow);
            }
        }
    }
}
