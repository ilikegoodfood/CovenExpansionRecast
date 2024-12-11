using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Act_RaiseLucidity : Assets.Code.Action
    {
        public Act_RaiseLucidity(Location loc) : base(loc)
        {

        }

        public override string getName()
        {
            return "Encourage Lucidity";
        }

        public override string getShortDesc()
        {
            return $"Increases Lucidity by {location.map.param.act_innoculateEffect}. Cannot be poerfomed by the insane.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.lucid.png");
        }

        public override double getUtility(SettlementHuman humanSettlement, Person ruler, List<ReasonMsg> reasons)
        {
            double utility = base.getUtility(humanSettlement, ruler, reasons);
            double plagueCharge = 0.0;
            double lucidityCharge = 0.0;

            foreach (Property pr in humanSettlement.location.properties)
            {
                if (pr is Pr_MagicPlague plague)
                {
                    plagueCharge += plague.charge;
                }
                else if (pr is Pr_Lucidity lucidity)
                {
                    lucidityCharge += lucidity.charge;
                }
            }

            if (plagueCharge > 0.0)
            {
                utility += plagueCharge;
                reasons?.Add(new ReasonMsg("Level of Psychogenic Illness", plagueCharge));
            }

            if (lucidityCharge > 0.0)
            {
                lucidityCharge *= 0.5;
                utility -= lucidityCharge;
                reasons?.Add(new ReasonMsg("Existing lucidity", -lucidityCharge));
            }

            return utility;
        }

        public override bool valid(Person ruler, SettlementHuman settlementHuman)
        {
            return !ruler.traits.Any(t => t is T_Insane);
        }

        public override void complete()
        {
            foreach (Property property in location.properties)
            {
                if (property is Pr_Lucidity lucidity)
                {
                    lucidity.charge += map.param.act_innoculateEffect;
                }
            }
        }

        public override int[] getNegativeTags()
        {
            return new int[]
            {
                Tags.DISEASE,
                Tags.MADNESS
            };
        }
    }
}
