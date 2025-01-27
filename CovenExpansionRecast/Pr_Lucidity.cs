using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Pr_Lucidity : Property
    {
        public Pr_Lucidity(Location loc)
            : base(loc)
        {
            charge = 1.0;

            stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override string getName()
        {
            return "Lucidity";
        }

        public override string getDesc()
        {
            return "This modifier is created by psychogenic illness. For every 50 charge this modifier achieves, the illness will lose 1 charge every turn. Lucidity is lost over time if the location isn't exposed to illness.";
        }

        public override Sprite getSprite(World world)
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Lucid.png");
        }

        public override void turnTick()
        {
            if (charge > 300.0)
            {
                charge = 300.0;
            }

            influences.Add(new ReasonMsg("Constant Decrease", -2.0));

            int deltaCharge = (int)(charge / 50.0);
            foreach (Property property in location.properties)
            {
                if (property is Pr_MagicPlague)
                {
                    property.influences.Add(new ReasonMsg("Lucidity", -deltaCharge));
                }
            }
        }

        public static void addToProperty(string cause, double amount, Location loc)
        {
            Pr_Lucidity lucidity = (Pr_Lucidity)loc.properties.FirstOrDefault(pr => pr is Pr_Lucidity);
            if (lucidity == null)
            {
                lucidity = new Pr_Lucidity(loc);
                loc.properties.Add(lucidity);
            }

            lucidity.influences.Add(new ReasonMsg(cause, amount));
        }
    }
}
