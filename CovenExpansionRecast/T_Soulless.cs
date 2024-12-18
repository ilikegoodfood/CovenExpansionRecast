using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Soulless : Trait
    {
        public double shadowGrowth = 0.003;

        public override string getName()
        {
            return "Soulless";
        }

        public override string getDesc()
        {
            return "This persons soul has been stolen causing them to slowly gain shadow over time.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person)
            {
                p.traits.Remove(this);
                return;
            }

            p.shadow += shadowGrowth;
            if (p.shadow > 1.0)
            {
                p.shadow = 1.0;
            }
        }
    }
}
