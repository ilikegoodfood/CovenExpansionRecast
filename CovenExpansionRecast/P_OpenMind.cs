using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class P_OpenMind : Power
    {
        public int Cost = 0;

        public P_OpenMind(Map map)
            : base(map)
        {
            
        }

        public override string getName()
        {
            return "Eldritch Command";
        }

        public override string getDesc()
        {
            return "Chooses a local action for a ruler or a quest for a hero. Rulers immediatly change their local action to the chosen action while heroes will perform the chosen quest as their next action if possible. You cannot choose local actions added by Vinnervas gifts or special actions gained through outside sources like The Hunger. The first use of this power each turn is free.";
        }

        public override string getFlavour()
        {
            return "There is no safety for them now. Their minds are laid bare before you, plain as day; all that's left is to give an order.";
        }

        public override string getRestrictionText()
        {
            return "Must be cast on a hero or ruler with a Collected Mind curse. Cannot target the chosen one. Can only be used once per turn.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Mind.png");
        }

        public override bool validTarget(Unit unit)
        {
            return (unit.person?.traits.Any(t => t is T_CollectedMind) ?? false);
        }

        public override bool validTarget(Location loc)
        {
            return (loc.person()?.traits.Any(t => t is T_CollectedMind) ?? false);
        }


    }
}
