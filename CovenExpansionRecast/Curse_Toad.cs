using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Curse_Toad : Curse
    {
        public override string getName()
        {
            return "Curse of Toad";
        }

        public override string getDesc()
        {
            return "This family has been transformed into toads.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person || p.unit == null || !(p.unit is UA) || p.unit is UAEN || p.unit.isDead || p.unit.isCommandable())
            {
                return;
            }

            UAEN_Toad toad = new UAEN_Toad(p.unit.location, p.map.soc_dark, p);
            p.unit.isDead = true;
            p.unit.location.units.Remove(p.unit);
            p.map.units.Remove(p.unit);

            toad.SubsumedUnit = p.unit;
            p.unit = toad;
            p.map.units.Add(toad);
            p.unit.location.units.Add(toad);
        }
    }
}
