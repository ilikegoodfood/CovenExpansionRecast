using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_FictitiousBonds : Trait
    {
        public House House;

        public T_FictitiousBonds(House house)
        {
            House = house;
        }

        public override string getName()
        {
            return "Fictitious Bonds";
        }

        public override string getDesc()
        {
            return $"While in a location with a ruler of house {House?.name ?? "ERROR: No House"}, the location loses 1 <b>security</b>";
        }

        public override int getMaxLevel()
        {
            return 1;
        }

        public override void turnTick(Person p)
        {
            if  (p.house != House)
            {
                p.traits.Remove(this);
                if (p.map.burnInComplete && p.map.world.displayMessages  && p.unit != null && p.unit.isCommandable())
                {
                    p.map.addMessage($"After changing allegiance to a different house, {p.getFullName()} has lost {getName()} with House {House.name}.", 0.5, false, p.getLocation().hex);
                }
            }
        }

        public override int getSecurityChange(Unit u, SettlementHuman settlementHuman)
        {
            if (settlementHuman.ruler != null && settlementHuman.ruler.house == House)
            {
                return -1;
            }

            return 0;
        }

        public override int getSecurityChangeFromRuler(SettlementHuman settlementHuman, Person ruler)
        {
            if (ruler.house == House)
            {
                return -1;
            }

            return 0;
        }
    }
}
