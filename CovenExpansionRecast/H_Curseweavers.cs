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

        public override string getName()
        {
            return "Curseweavers";
        }

        public override string getDesc()
        {
            return "Witches will place curses on other acolytes that perform challenges in locations following this holy order.";
        }

        public override int getMaxPositiveInfluence()
        {
            return 0;
        }

        public override int getMaxNegativeInfluence()
        {
            return -2;
        }

        public override double addUtility(Challenge c, UA ua, List<ReasonMsg> msgs)
        {
            double val = 0.0;
            if (c is Ch_H_CurseIntrudingAcolyte)
            {
                val = status * -50.0;
                msgs?.Add(new ReasonMsg($"Tenet: {getName()}", val));
            }
            return val;
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
                if (curseAcolyte.Order == order)
                {
                    if (curseAcolyte.Cooldown > 0)
                    {
                        curseAcolyte.Cooldown--;
                    }
                }
                else if (curseAcolyte.Order.isGone())
                {
                    settlementHuman.customChallenges.Remove(curseAcolyte);
                }
            }
        }
    }
}
