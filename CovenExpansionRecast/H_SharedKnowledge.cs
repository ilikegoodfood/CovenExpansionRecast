using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class H_SharedKnowledge : HolyTenet
    {
        public H_SharedKnowledge(HolyOrder us)
            : base(us)
        {

        }

        public override string getName()
        {
            return "Shared Wisdom";
        }

        public override string getDesc()
        {
            return "While at a temple of this holy order, all agents will gain a bonus lore for each point of elder influence, or will lose lore for human influence.";
        }

        public override int getMaxPositiveInfluence()
        {
            return 3;
        }

        public override bool structuralTenet()
        {
            return false;
        }

        public override void turnTickTemple(Sub_Temple temple)
        {
            if (status == 0)
            {
                return;
            }

            foreach (Unit unit in temple.settlement.location.units)
            {
                if (unit.person == null)
                {
                    continue;
                }

                T_StatTempLore tempLore = new T_StatTempLore(1, -status);
                unit.person.receiveTrait(tempLore);
            }
        }
    }
}
