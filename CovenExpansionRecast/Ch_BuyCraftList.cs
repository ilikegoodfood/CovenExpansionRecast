using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Ch_BuyCraftList : Ch_BuyItem
    {
        public int cost = 5;

        public Ch_BuyCraftList(Location loc)
            : base(loc)
        {
            onSale = new I_CraftList(loc.map);
            
        }

        public override string getName()
        {
            return $"Buy {onSale.getName()}";
        }

        public override string getDesc()
        {
            return $"Buys a {onSale.getName()}, at the cost of {cost} gold. \n{onSale.getShortDesc()}";
        }

        public override string getRestriction()
        {
            if (!CovensCore.Opt_Curseweaving)
            {
                return $"The mod option for curseweaving is disabled. Enable it in the Configure Mods menu if you wish to learn and use curseweaving.";
            }

            return $"Costs {cost} <b>gold</b>";
        }

        public override bool valid()
        {
            return CovensCore.Opt_Curseweaving;
        }

        public override bool validFor(UA ua)
        {
            return ua.person.gold >= cost;
        }

        public override double getUtility(UA ua, List<ReasonMsg> msgs)
        {
            return 0.0;
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override void complete(UA u)
        {
            u.person.gold -= cost;
            u.person.gainItem(new I_CraftList(u.map));
        }
    }
}
