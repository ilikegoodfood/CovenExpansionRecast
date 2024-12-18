using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Curse_Wanderer : Curse
    {
        public override string getName()
        {
            return "Insatiable Wanderlust";
        }

        public override string getDesc()
        {
            return "This family suffers from an insatiable longing for the new and unknown. Heroes from this family will seek out progressively stranger quests, never satisfied with doing the same thing twice.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person || (p.unit != null && p.unit.isCommandable()))
            {
                return;
            }

            T_Wanderer wanderer = (T_Wanderer)p.traits.FirstOrDefault(t => t is T_Wanderer);
            if (wanderer == null)
            {
                wanderer = new T_Wanderer();
                p.receiveTrait(wanderer);
            }
        }
    }
}
