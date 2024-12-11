using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class H_Curseweavers : HolyTenet
    {
        public H_Curseweavers(HolyOrder us)
            : base(us)
        {

        }

        public override void turnTickSettlement(SettlementHuman settlementHuman)
        {
            List<Ch_H_CurseIntrudingAcolyte> curseAcolyteChallenges = settlementHuman.customChallenges.OfType<Ch_H_CurseIntrudingAcolyte>().ToList();

            if (curseAcolyteChallenges.Count == 0)
            {
                settlementHuman.customChallenges.Add(new Ch_H_CurseIntrudingAcolyte(settlementHuman.location, order));
                return;
            }

            foreach (Ch_H_CurseIntrudingAcolyte curseAcolyte in curseAcolyteChallenges)
            {
                if (curseAcolyte.Cooldown > 0)
                {
                    curseAcolyte.Cooldown--;
                }
            }
        }
    }
}
