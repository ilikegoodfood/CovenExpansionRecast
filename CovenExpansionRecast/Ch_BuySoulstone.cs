using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Ch_BuySoulstone : Ch_BuyItem
    {
        public int cost = 15;
        public Ch_BuySoulstone(Location loc)
            : base(loc)
        {
            onSale = new I_Soulstone(loc.map);
        }

        public override string getName()
        {
            return "Buy " + onSale.getName();
        }

        public override string getDesc()
        {
            return $"Buys a {onSale.getName()}, at the cost of {cost} gold. \n{onSale.getShortDesc()}";
        }

        public override string getRestriction()
        {
            return $"Costs {cost} <b>gold</b>";
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
            u.person.gainItem(new I_Soulstone(u.map));
        }
    }
}
