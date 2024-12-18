using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_MurderOfCrows : Trait
    {
        public override string getName()
        {
            return "Murder of Crows";
        }

        public override string getDesc()
        {
            return $"Once every {World.staticMap.param.trait_commandOfVerminPeriod} turns, and empty minion slot is automatically filled with a 'Crow' minion (1 HP, 2 Attack, 1 Command Cost).";
        }

        public override int getMaxLevel()
        {
            return 1;
        }

        public override void turnTick(Person p)
        {
            if (p.map.turn % p.map.param.trait_commandOfVerminPeriod != 0 || !(p.unit is UA ua) || ua.getCurrentlyUsedCommand() >= ua.getStatCommandLimit())
            {
                return;
            }

            for (int i = 0; i < ua.minions.Length; i++)
            {
                if (ua.minions[i] == null)
                {
                    ua.minions[i] = new M_Crow(ua.map);
                    break;
                }
            }
        }
    }
}
