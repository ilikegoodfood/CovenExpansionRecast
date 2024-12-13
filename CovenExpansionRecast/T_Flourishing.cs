using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Flourishing : Trait
    {
        public override string getName()
        {
            return "Forced Flourishing";
        }

        public override string getDesc()
        {
            return "Life follows this persons footsteps, increasing the population of wherever they are.";
        }

        public override void turnTick(Person p)
        {
            if (p.getLocation().settlement is SettlementHuman humanSettlement)
            {
                humanSettlement.population++;
            }
        }
    }
}
