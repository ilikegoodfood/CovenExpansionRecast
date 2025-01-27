using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class H_Soulweavers : HolyTenet
    {
        public H_Soulweavers(HolyOrder us)
            : base(us)
        {

        }

        public override string getName()
        {
            return "Soulweavers";
        }

        public override string getDesc()
        {
            return "Enables soul transposition. Bring two filled soulstones to a coven or temple to combine them into an item. As elder alignment increases, you may purchase Transposing Scrolls that contain the transposing recipes for nearly every item.";
        }

        public override int getMaxPositiveInfluence()
        {
            return 0;
        }

        public override int getMaxNegativeInfluence()
        {
            return -2;
        }

        public override bool structuralTenet()
        {
            return false;
        }

        public override void turnTickTemple(Sub_Temple temple)
        {
            Ch_BuyCraftList buyCraftList = (Ch_BuyCraftList)temple.settlement.customChallenges.FirstOrDefault(ch => ch is Ch_BuyCraftList);
            if (status > -2)
            {
                if (buyCraftList != null)
                {
                    temple.settlement.customChallenges.Remove(buyCraftList);
                }
            }
            else if (buyCraftList == null)
            {
                temple.settlement.customChallenges.Add(new Ch_BuyCraftList(temple.settlement.location));
            }
        }
    }
}
