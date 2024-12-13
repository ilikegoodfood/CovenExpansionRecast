using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_LivingShadow : Trait
    {
        public override string getName()
        {
            return "Living Shadow";
        }

        public override string getDesc()
        {
            return "The sins of this family have manifested themselves as a living spiteful thing. It watches over them, guiding them to greater ruin.";
        }

        public override void turnTick(Person p)
        {
            if (p.unit != null && p.unit.isCommandable())
            {
                p.traits.Remove(this);
            }
        }
    }
}
