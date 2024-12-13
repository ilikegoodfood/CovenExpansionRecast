using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Curse_Flourish : Curse
    {
        public override string getName()
        {
            return "Forced Flourishing";
        }

        public override string getDesc()
        {
            return "Life follows the footsteps of this family, increasing the population of wherever they are.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person)
            {
                return;
            }

            T_Flourishing flourishing = (T_Flourishing)p.traits.FirstOrDefault(t => t is T_Flourishing);
            if (flourishing == null)
            {
                flourishing = new T_Flourishing();
                p.receiveTrait(flourishing);
            }
        }
    }
}
