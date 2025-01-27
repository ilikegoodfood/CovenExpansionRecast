using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_PhthisicalPoisoned : Trait
    {
        public int Activated = 0;

        public override string getName()
        {
            if (Activated > 0)
            {
                if (Activated < level)
                {
                    return $"Will be poisoned again ({Activated})";
                }
                else
                {
                    return $"Disfigured by phthisical poison ({Activated})";
                }
            }

            return "Will be poisoned";
        }

        public override string getDesc()
        {
            if (Activated > 0)
            {
                if (Activated < level)
                {
                    return $"Permanently decreases all stats by ({Activated}). Caused by an agent poisoning the hero with a phthisical vial. When the hero next rests they will be poisoned again worsening their condition.";
                }
                else
                {
                    return $"Permanently decreases all stats by ({Activated}). Caused by an agent poisoning the hero with a phthisical vial.";
                }
            }

            return "When the hero rests they will be poisoned, permanently decreasing all stats by 1.";
        }

        public override void turnTick(Person p)
        {
            if (p.unit == null || !(p.unit.task is Task_PerformChallenge performChallenge) || !(performChallenge.challenge is Ch_Rest))
            {
                return;
            }

            Activated = level;
            p.unit.task = new Task_Disrupted(2 * p.map.param.ch_poisonHeroDisruptDur);
            p.map.addMessage(p.unit.getName() + " has been poisoned with phthisical poison while resting", 0.7, true, p.unit.location.hex);
        }

        public override int getMaxLevel()
        {
            return 10;
        }

        public override int getMightChange()
        {
            return -Activated;
        }

        public override int getLoreChange()
        {
            return -Activated;
        }

        public override int getIntrigueChange()
        {
            return -Activated;
        }

        public override int getCommandChange()
        {
            return -Activated;
        }
    }
}
